using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField, Tooltip("�� ƽ�� �� ������")]
    private ElementDamage damage;
    [SerializeField, Tooltip("������ �ֱ�")]
    private float damageInterval = 0.2f;
    private float currentDamageTick = 0.0f;

    [SerializeField, Tooltip("���� �ð�")]
    private float duration = 2.0f;
    private float lifeTime;

    [SerializeField, Tooltip("�θ� Ʈ������(ũ�� ������)")]
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
    /// �� ƽ���� ������ �ֱ�
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

        // hit collider ����
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
