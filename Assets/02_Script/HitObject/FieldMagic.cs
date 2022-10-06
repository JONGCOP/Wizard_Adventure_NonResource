using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 범위 공격
/// 주의 : 해당 프레임에만 공격하는 방식이 아닌 지속시간 동안 범위 안에 들어가면 공격 판정
/// </summary>
[RequireComponent(typeof(Collider))]
public class FieldMagic : Magic
{
    [SerializeField, Range(0.1f, 5.0f), Tooltip("히트 판정이 지속되는 시간")]
    private float hitDuration = 2.0f;
    [SerializeField, Range(0.1f, 5.0f), Tooltip("마법이 남아있는 시간 (파티클 등 남아있는거 기다림)")] 
    private float remainTime = 3.0f;

    private float hitLifeTime;
    private float remainLifeTime;

    [SerializeField]
    private ElementDamage elementDamage;

    private Collider hitBox;

    private void Awake()
    {
        hitBox = GetComponent<Collider>();
        if (hitDuration > remainTime)
        {
            Debug.LogWarning("hit Duration can't longer than remain time");
        }
    }

    private void OnEnable()
    {
        hitBox.enabled = true;

        hitLifeTime = hitDuration;
        remainLifeTime = remainTime;

        ReviseTransform();
    }

    private void ReviseTransform()
    {
        RaycastHit hit;
        // 위치 보정 - 무조건 바닥에 생성되도록 바닥으로 Ray를 쏴서 생성 위치 판정
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 
                100, 1 << LayerMask.NameToLayer("Default")))
        {
            transform.position = hit.point;
        }
        else
        {
            print("invalid field position. can't revise position");
            return;
        }

        // 각도 보정 - ray 기준 
        //transform.up = hit.normal;
    }

    private void FixedUpdate()
    {
        hitLifeTime -= Time.fixedDeltaTime;
        // 히트박스 유지 시간동안은 
        if (hitLifeTime < 0 && hitBox.enabled)
        {
            hitBox.enabled = false;
        }

        remainLifeTime -= Time.fixedDeltaTime;
        if (remainLifeTime < 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("hit to " + other.name);

        var status = other.GetComponent<CharacterStatus>();
        if (status)
        {
            status.TakeDamage(elementDamage);
        }
    }
}
