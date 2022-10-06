using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField, Tooltip("한 틱당 줄 데미지")]
    private ElementDamage damage;
    [SerializeField, Tooltip("데미지 주기")]
    private float damageInterval = 0.2f;
    private float currentDamageTick = 0.0f;

    [SerializeField, Tooltip("지속 시간")]
    private float duration = 2.0f;
    private float lifeTime;

    [SerializeField, Tooltip("부모 트랜스폼(크기 조정용)")]
    private Transform originTransform;

    private CapsuleCollider hitCollider;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        hitCollider = GetComponent<CapsuleCollider>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        lifeTime = duration;
    }

    private void FixedUpdate()
    {
        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime < 0)
        {
            gameObject.SetActive(false);
        }

        if (currentDamageTick < 0.0f)
        {
            currentDamageTick = damageInterval;
        }
        currentDamageTick -= Time.fixedDeltaTime;
    }

    /// <summary>
    /// 매 틱마다 데미지 주기
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (currentDamageTick > 0)
        {
            return;
        }

        var status = other.GetComponent<CharacterStatus>();
        if (status)
        {
            status.TakeDamage(damage);
        }
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPos);

        // hit collider 조정
        float colliderSize = 1f;
        if (originTransform != null)
        {
            colliderSize = 1 / originTransform.localScale.z;
        }

        var height = (Vector3.Distance(transform.position, targetPos) * colliderSize) + 0.5f;
        var center = (height * 0.5f) - 0.25f;
        hitCollider.height = height;
        hitCollider.center = Vector3.forward * center;
    }
}
