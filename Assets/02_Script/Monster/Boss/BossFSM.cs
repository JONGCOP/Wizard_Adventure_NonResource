using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� 1������ FSM
/// ������ ���� �������� �ʰ� ���ϵ��� Ȯ�������� ����ؼ� �����Ѵ�
/// (�� ���ϵ� ���̿� ��Ÿ���� ����)
/// �ۼ��� - ����ö
/// </summary>
public partial class BossFSM : MonoBehaviour
{
    protected enum BossState
    {
        Idle,
        Move,
        Attack,
        //Damaged,
        Die
    }

    /// <summary>
    /// ���� ��ų ���� ���� ����ü
    /// </summary>
    [System.Serializable]
    public class BossSkillData
    {
        [SerializeField, Tooltip("��ų �ּ� ��Ÿ�� / �ִ� ��Ÿ��")]
        private Vector2 cooldown = new Vector2(15, 30);
        [SerializeField, Tooltip("���� ��ٿ�(���⿡ �����ϸ� �ʱ� ��ٿ�)")]
        private float currentCooldown = 0;

        [SerializeField, Tooltip("���� �ൿ���� ��ٸ��� �ð�")]
        private float nextActionDelay = 1.5f;
        
        public Vector2 Cooldown => cooldown;
        
        public float CurrentCooldown
        {
            get => currentCooldown;
            set => currentCooldown = value;
        }
        public float NextActionDelay => nextActionDelay;
    }

    [SerializeField, Tooltip("�⺻���� ȸ�� �ӵ�")]
    private float baseRotationSpeed = 30.0f;
    private float currentRotationSpeed;
    private bool useRotateToward = true;

    protected BossState state;

    // ���ݴ��: Player
    private Transform attackTarget;
    

    protected Animator animator;
    private CharacterStatus charStatus;
    private AudioSource audioSource;

    private int phase = 0;
    public int Phase
    {
        get => phase;
        private set
        {
            phase = value;
            animator.SetInteger(phaseID, phase);
        }
    }

    [Header("Common Sound")] 
    [SerializeField] private AudioClip damagedSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Entrance Sound")]
    [SerializeField] private AudioClip appearanceSound;
    [SerializeField] private AudioClip enteredSound;
    [SerializeField] private AudioClip patternStartSound;
    [SerializeField] private Knockback enteredKnockbackPrefab;

    private static readonly float actionTime = 0.1f;
    private static readonly WaitForSeconds actionWS = new WaitForSeconds(actionTime);

    #region AnimationID
    private static readonly int isPlayerDeadID = Animator.StringToHash("isPlayerDead");
    private static readonly int phaseID = Animator.StringToHash("Phase");
    private static readonly int skillStateID = Animator.StringToHash("SkillState");
    private static readonly int isActionDelayID = Animator.StringToHash("IsActionDelay");
    private static readonly int isShockedID = Animator.StringToHash("isShocked");
    private static readonly int isDieID = Animator.StringToHash("isDie");
    #endregion

    public event Action onDeathFinished;

    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        charStatus = GetComponent<CharacterStatus>();

        audioSource = GetComponent<AudioSource>();

        charStatus.onSpeedChenge += OnSpeedChange;
        charStatus.onShocked += OnShocked;
        charStatus.onDead += OnDead;
    }

    private void Start()
    {
        attackTarget = GameManager.player.transform;
    }

    #region Enter
    public void Appearance()
    {
        audioSource.PlayOneShot(appearanceSound);
    }

    public void StartKnockback()
    {
        animator.SetInteger(skillStateID, -1);
        audioSource.PlayOneShot(enteredSound);
    }

    private void EnterKnockback()
    {
        Instantiate(enteredKnockbackPrefab, transform.position + Vector3.up, Quaternion.identity);
    }

    public void StartFSM()
    {
        Sequence s = DOTween.Sequence();

        s.AppendCallback(() =>
        {
            // ��� ���ϱ�
            audioSource.PlayOneShot(patternStartSound);
        });
        s.AppendInterval(3.0f);
        s.onComplete = () =>
        {
            // ���� ����
            Phase = 1;
            animator.SetInteger(skillStateID, 0);

            Skill1_Init();
        };
    }


    #endregion

    private void Update()
    {
        if (phase == 1)
        {
            DecreaseCooldown();
        }

        if (useRotateToward)
        {
            RotateTowards(attackTarget, currentRotationSpeed);
        }
    }

    private void NextActionDelay()
    {
        var skillState = animator.GetInteger(skillStateID);
        audioSource.PlayOneShot(damagedSound);
        if (actionDelayCoroutine == null)
        {
            actionDelayCoroutine = StartCoroutine(IEWaitActionDelay(phase1SkillDatas[skillState].NextActionDelay));
        }
    }

    private IEnumerator IEWaitActionDelay(float delay)
    {
        useRotateToward = false;
        animator.SetBool(isActionDelayID, true);
        yield return new WaitForSeconds(delay);
        animator.SetBool(isActionDelayID, false);
        actionDelayCoroutine = null;
        useRotateToward = true;
    }

    // ȸ�� �ӵ��� ���� ȸ���ϴ� �Լ�
    private void RotateTowards(Transform target, float rotSpeed)
    {
        var targetPos = target.position;
        // Y�� ȸ���� �ϵ��� y���� ���� ����
        targetPos.y = transform.position.y;
        var toForward = (targetPos - transform.position).normalized;
        var toForwardRot = Quaternion.LookRotation(toForward);
        var newRot = Quaternion.RotateTowards(transform.rotation, toForwardRot, rotSpeed * Time.deltaTime);
        transform.rotation = newRot;
    }

    public void ResetFSM()
    {
        animator.SetBool(isPlayerDeadID, false);
    }

    #region Hit Reaction

    // ���Ͱ� �ñ� ���ظ� �Ծ��� �� �ִϸ��̼� �ӵ� ���� �� �ӵ� ����
    public void OnSpeedChange(float freezeSpeed)
    {
        animator.speed = freezeSpeed;

    }

    public void OnShocked()
    {
        print("Shock Animation");
        // ������ ����Ѵ�
        animator.SetTrigger(isShockedID);
        audioSource.Stop();
        audioSource.PlayOneShot(damagedSound);
        StopAllCoroutines();

        phase1Skill5.chargingEffect.gameObject.SetActive(false);
        phase1Skill5.chargingSphere.gameObject.SetActive(false);
        charStatus.ShockResistPercent = 100;
    }
    #endregion

    protected virtual void OnDead()
    {
        // ��������
        print("Boss Dead");
        animator.SetTrigger(isDieID);
        audioSource.PlayOneShot(deathSound);
    }

    public virtual void OnDeathFinished()
    {
        //print("Death Finished");
        onDeathFinished?.Invoke();
        gameObject.SetActive(false);
    }
}
