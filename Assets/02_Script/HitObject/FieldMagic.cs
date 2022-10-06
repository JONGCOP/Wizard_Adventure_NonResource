using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����
/// ���� : �ش� �����ӿ��� �����ϴ� ����� �ƴ� ���ӽð� ���� ���� �ȿ� ���� ���� ����
/// </summary>
[RequireComponent(typeof(Collider))]
public class FieldMagic : Magic
{
    [SerializeField, Range(0.1f, 5.0f), Tooltip("��Ʈ ������ ���ӵǴ� �ð�")]
    private float hitDuration = 2.0f;
    [SerializeField, Range(0.1f, 5.0f), Tooltip("������ �����ִ� �ð� (��ƼŬ �� �����ִ°� ��ٸ�)")] 
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
        // ��ġ ���� - ������ �ٴڿ� �����ǵ��� �ٴ����� Ray�� ���� ���� ��ġ ����
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

        // ���� ���� - ray ���� 
        //transform.up = hit.normal;
    }

    private void FixedUpdate()
    {
        hitLifeTime -= Time.fixedDeltaTime;
        // ��Ʈ�ڽ� ���� �ð������� 
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
