using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 보스 1페이즈 FSM
/// 보스는 직접 공격하지 않고 패턴들을 확률적으로 사용해서 공격한다
/// (각 패턴들 사이엔 쿨타임이 있음)
/// 작성자 - 차영철
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
    /// 보스 스킬 정의 정보 구조체
    /// </summary>
    [System.Serializable]
    public class BossSkillData
    {
        [SerializeField, Tooltip("스킬 최소 쿨타임 / 최대 쿨타임")]
        private Vector2 cooldown = new Vector2(15, 30);
        [SerializeField, Tooltip("현재 쿨다운(여기에 기입하면 초기 쿨다운)")]
        private float currentCooldown = 0;

        [SerializeField, Tooltip("다음 행동까지 기다리는 시간")]
        private float nextActionDelay = 1.5f;
        
        public Vector2 Cooldown => cooldown;
        
        public float CurrentCooldown
        {
            get => currentCooldown;
            set => currentCooldown = value;
        }
        public float NextActionDelay => nextActionDelay;
    }

    [SerializeField, Tooltip("기본적인 회전 속도")]
    private float baseRotationSpeed = 30.0f;
    private float currentRotationSpeed;
    private bool useRotateToward = true;

    protected BossState state;

    // 공격대상: Player
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
            // 대사 말하기
            audioSource.PlayOneShot(patternStartSound);
        });
        s.AppendInterval(3.0f);
        s.onComplete = () =>
        {
            // 패턴 시작
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

    // 회전 속도에 맞춰 회전하는 함수
    private void RotateTowards(Transform target, float rotSpeed)
    {
        var targetPos = target.position;
        // Y축 회전만 하도록 y값을 같게 설정
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

    // 몬스터가 냉기 피해를 입었을 때 애니메이션 속도 감속 및 속도 복구
    public void OnSpeedChange(float freezeSpeed)
    {
        animator.speed = freezeSpeed;

    }

    public void OnShocked()
    {
        print("Shock Animation");
        // 패턴을 취소한다
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
        // 죽음상태
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
