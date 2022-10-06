using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해당 방향으로 나아가는 투사체형 오브젝트
/// 작성자 - 차영철
/// </summary>
public class Projectile : Magic
{
    [SerializeField, Tooltip("투사체 이동속도")]
    protected float moveSpeed = 4.5f;
    [SerializeField, Tooltip("투사체 사정거리")]
    private float range = 5f;
    // 투사체가 얼마나 오래 유지되는지
    private float lifetime;

    // 데미지 등등 정보 설정
    [SerializeField, Tooltip("데미지 및 속성 정보")] 
    private ElementDamage elementDamage;

    [Header("VFX")]
    [SerializeField, Tooltip("투사체 맞았을 때 효과 (VFX, SFX 포함)")]
    private ParticleSystem hitEffectPrefab;

    [Header("SFX")]
    [SerializeField, Tooltip("발사할 때 나는 소리")]
    private AudioClip shootSound;
    [SerializeField, Tooltip("맞았을 때 나는 소리")]
    private AudioClip hitSound;

    protected Rigidbody rb;

    public event Action onDestroy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // 초기 탄환 폭발 생성
        PoolSystem.Instance.InitPool(hitEffectPrefab, 4);
    }

    private void OnEnable()
    {
        lifetime = range / moveSpeed;
    }

    private void OnDisable()
    {
        onDestroy?.Invoke();
        onDestroy = null;
    }

    /// <summary>
    /// Projectile 초기화 시 속도(방향, 속력)를 지정하는 함수
    /// </summary>
    public void Shoot(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.forward = direction;
        rb.velocity = direction * moveSpeed;
        rb.angularVelocity = Vector3.zero;
    }

    public override void StartMagic()
    {
        rb.velocity = transform.forward * moveSpeed;
        rb.angularVelocity = Vector3.zero;
    }

    protected virtual void FixedUpdate()
    {
        // 수명 제어
        lifetime -= Time.fixedDeltaTime;
        if (lifetime < 0)
        {
            Destroy();
        }

        // 중력 받는 경우 투사체 공격도 회전시켜야함
        if (rb.useGravity)
        {
            transform.forward = rb.velocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print(collision.gameObject.name);
        // 캐릭터 스탯이 있는 경우에만 데미지 및 속성 효과 주기
        var status = collision.gameObject.GetComponent<CharacterStatus>();
        if (status)
        {
            status.TakeDamage(elementDamage);
        }

        // 삭제
        Destroy();
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
        var hitEffect = PoolSystem.Instance.GetInstance<ParticleSystem>(hitEffectPrefab);
        hitEffect.transform.position = transform.position;
    }
}
