using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �̸��� State ���� �� �ִϸ��̼�, �׸��� ȿ����
/// �ۼ��� - ������
/// </summary>

public class EnemyFSM : MonoBehaviour
{
    // ���ʹ� Player Layer�� �����Ѵ�
    protected static int playerLayer;
    protected enum EnemyState
    {
        Idle,
        Move,
        Attack,
        //Damaged,
        Die
    }

    protected EnemyState state;

    
    public Transform attackTarget;
    private CharacterStatus targetStatus;
    // ���ݴ��: Player
    private float dist;
    public float chaseDistance;
    public float attackDistance;
    protected NavMeshAgent agent;
    protected Animator animator;
    public CharacterStatus charStatus;
    public ElementDamage elementDamage;
    public GameObject deadFXFactory;
    public GameObject fxPos;
    private bool moveLock;
    private bool checkDead = false;
    private bool initChaseTrigger = true; // �߰ݹ��� �ۿ��� �÷��̾�� ���ظ� �Ծ��� �� �ڵ����� �÷��̾ �߰��ϵ��� �ϱ�����.

    #region(AudioClip)
    public AudioSource audioSource;
    [Header("AudioClip")]
    public AudioClip[] idle;
    public AudioClip footStep_1;
    public AudioClip footStep_2;
    [Tooltip("�÷��̾ ���� �޷����鼭 ���� �Ҹ�")]
    public AudioClip chaseGrowl;
    public AudioClip attack;
    [Tooltip("�����ϸ鼭 ���� �Ҹ�")]
    public AudioClip attackGrowl;
    [Tooltip("Ȱ���� ��� �� ���� �Ҹ�")]
    public AudioClip arrowCharge;
    [Tooltip("�������¿��� ���� �Ҹ�")]
    public AudioClip shocked;
    [Tooltip("���Ͱ� �����鼭 ���� �Ҹ�")]
    public AudioClip deadGrowl;
    [Tooltip("���Ͱ� �����鼭 ���� ȯ����")]
    public AudioClip dead;
    [Tooltip("���Ͱ� ���� �� ��Ÿ���� ��ƼŬ ȿ����")]
    public AudioClip deadFXSound;
    #endregion

    private static readonly int isPlayerDeadID = Animator.StringToHash("isPlayerDead");

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        charStatus = GetComponent<CharacterStatus>();

        attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
        targetStatus = attackTarget.GetComponent<CharacterStatus>();
        audioSource = GetComponent<AudioSource>();

        charStatus.onSpeedChenge += OnFreeze;
        charStatus.onShocked += OnShocked;
        charStatus.onDead += OnDead;
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateState());
        state = EnemyState.Idle;
        agent.isStopped = false;
    }

    private void Update()
    {
        dist = Vector3.Distance(this.transform.position, attackTarget.transform.position);
        //Debug.Log(dist);
        if(state == EnemyState.Attack)
        {
            var targetPos = attackTarget.position;
            targetPos.y = transform.position.y;
            transform.forward = (targetPos - transform.position).normalized;
        }
    }

    IEnumerator UpdateState()
    {
        print($"{gameObject.name} - Update State");
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Move:
                    Move();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ResetFSM()
    {
        animator.SetBool(isPlayerDeadID, false);
        initChaseTrigger = true;
        agent.isStopped = true;
        moveLock = false;
        state = EnemyState.Idle;
    }

    void Idle()
    {
        // Enemy�� Player�� �Ÿ��� �����ϰ�, �߰� �Ÿ� �̳��� Move State�� ��ȯ�Ѵ�.
        if (dist <= chaseDistance)
        {
            ChaseGrowl();
            initChaseTrigger = false; // �������� ���� �� ���� ȣ����� �ʵ��� false�� ��ȯ
            state = EnemyState.Move;
        }
        if (this.charStatus.CurrentHp != this.charStatus.maxHp && initChaseTrigger == true)
        {
            ChaseGrowl();
            initChaseTrigger = false;
            state = EnemyState.Move;
        }
    }

    void Move()
    {
        if(dist > attackDistance && moveLock == false)
        {
            agent.isStopped = false;
            animator.SetBool("isMove", true);
            agent.SetDestination(attackTarget.transform.position);
        }
        else
        {
            animator.SetBool("isMove", false);
            state = EnemyState.Attack;
        }
    }

    #region Attack
    void Attack()
    {
        agent.isStopped = true;
        animator.SetBool("isAttack", true);
        if (dist>attackDistance)
        {
            animator.SetBool("isAttack", false);
            state = EnemyState.Move;
        }
        if (targetStatus.CurrentHp <= 0)
        {
            animator.SetBool("isPlayerDead", true);
            animator.SetBool("isAttack", false);
        }
    }

    public virtual void OnAttackHit()               
        // ����������(footman, warlord) ���� �ִϸ��̼� Ű�����ӿ� �������� �� �÷��̾�� �������� �������� �ϰ� �ʹ�.
    {
       if(agent.stoppingDistance >= dist)
        {
            // �÷��̾�� damage�� ������.
            targetStatus.TakeDamage(elementDamage);
            // �׸��� �ǰݻ��带 ����Ѵ�.
            AttackSound();
        }
    }
    #endregion

    #region Hit Reaction

    // ���Ͱ� �ñ� ���ظ� �Ծ��� �� ��� �ִϸ��̼� ��� �ӵ��� �������� ����� �ʹ�.
    public void OnFreeze(float freezeSpeed)               
    {
        animator.speed = freezeSpeed;
        
    }

    public void OnShocked()
    {
        //print("Shock Animation");
        moveLock = true;
        if(checkDead == true)
        {
            state = EnemyState.Die;
        }
        else
        {
            animator.SetTrigger("isShocked");
            StartCoroutine(ShockMoveLock());
        }
    }
    #endregion

    protected virtual void OnDead()
    {
        // ��������
        checkDead = true;
        agent.isStopped = true;
        state = EnemyState.Die;
        animator.SetTrigger("isDie");
        // �浹ü�� off�ϰ�ʹ�.
        GetComponent<Collider>().enabled = false;
    }

    public void DeadFx()
    {
        GameObject deadFX = Instantiate(deadFXFactory);
        deadFX.transform.position = fxPos.transform.position;
        ParticleSystem ps = deadFX.GetComponent<ParticleSystem>();
        ps.Stop();
        ps.Play();
        DeadFXSound();
    }

    public virtual void OnDeathFinished()
    {
        //print("Death Finished");
        //agent.isStopped = true;
        gameObject.SetActive(false);
    }

    // �������¿��� ���Ͱ� �÷��̾�� �ٰ����� �� �ϵ��� ������ �ʿ䰡 ����.
    private IEnumerator ShockMoveLock()
    {
        yield return new WaitForSeconds(3.0f);
        moveLock = false;
    }

    #region SFX

    public void IdleSound()
    {
        for(int i=0; i<idle.Length; i++)
        {
            audioSource.PlayOneShot(idle[i]);
            if(state != EnemyState.Idle)
            {
                audioSource.Stop();
            }
        }
    }

    public void FootStep_1()
    {
        audioSource.PlayOneShot(footStep_1);
    }

    public void FootStep_2()
    {
        audioSource.PlayOneShot(footStep_2);
    }

    public void ChaseGrowl()
    {
        audioSource.PlayOneShot(chaseGrowl);
    }

    public void AttackSound()
    {
        audioSource.PlayOneShot(attack);
        audioSource.PlayOneShot(attackGrowl);

    }

    public void ArrowCharge()
    {
        audioSource.PlayOneShot(arrowCharge);
    }

    public void ShockedSound()
    {
        audioSource.PlayOneShot(shocked);
    }

    public void DeadGrowl()
    {
        audioSource.PlayOneShot(deadGrowl);
    }

    public void DeadSound()
    {
        audioSource.PlayOneShot(dead);
    }

    public void DeadFXSound()
    {
        audioSource.PlayOneShot(deadFXSound);
    }

    #endregion


}
