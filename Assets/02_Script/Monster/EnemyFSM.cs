using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 쫄몹들 State 설정 및 애니메이션, 그리고 효과음
/// 작성자 - 성종현
/// </summary>

public class EnemyFSM : MonoBehaviour
{
    // 몬스터는 Player Layer만 공격한다
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
    // 공격대상: Player
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
    private bool initChaseTrigger = true; // 추격범위 밖에서 플레이어에게 피해를 입었을 때 자동으로 플레이어를 추격하도록 하기위함.

    #region(AudioClip)
    public AudioSource audioSource;
    [Header("AudioClip")]
    public AudioClip[] idle;
    public AudioClip footStep_1;
    public AudioClip footStep_2;
    [Tooltip("플레이어를 향해 달려오면서 내는 소리")]
    public AudioClip chaseGrowl;
    public AudioClip attack;
    [Tooltip("공격하면서 내는 소리")]
    public AudioClip attackGrowl;
    [Tooltip("활시위 당길 때 나는 소리")]
    public AudioClip arrowCharge;
    [Tooltip("경직상태에서 내는 소리")]
    public AudioClip shocked;
    [Tooltip("몬스터가 죽으면서 내는 소리")]
    public AudioClip deadGrowl;
    [Tooltip("몬스터가 죽으면서 나는 환경음")]
    public AudioClip dead;
    [Tooltip("몬스터가 죽을 때 나타나는 파티클 효과음")]
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
        // Enemy와 Player의 거리를 측정하고, 추격 거리 이내면 Move State로 전환한다.
        if (dist <= chaseDistance)
        {
            ChaseGrowl();
            initChaseTrigger = false; // 데미지를 입을 때 마다 호출되지 않도록 false로 전환
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
        // 근접몬스터의(footman, warlord) 공격 애니메이션 키프레임에 도달했을 때 플레이어에게 데미지를 입히도록 하고 싶다.
    {
       if(agent.stoppingDistance >= dist)
        {
            // 플레이어에게 damage를 입힌다.
            targetStatus.TakeDamage(elementDamage);
            // 그리고 피격사운드를 재생한다.
            AttackSound();
        }
    }
    #endregion

    #region Hit Reaction

    // 몬스터가 냉기 피해를 입었을 때 모든 애니메이션 재생 속도를 느려지게 만들고 싶다.
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
        // 죽음상태
        checkDead = true;
        agent.isStopped = true;
        state = EnemyState.Die;
        animator.SetTrigger("isDie");
        // 충돌체를 off하고싶다.
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

    // 경직상태에서 몬스터가 플레이어에게 다가오지 못 하도록 막아줄 필요가 있음.
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
