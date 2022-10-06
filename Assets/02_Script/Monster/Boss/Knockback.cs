using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ����ϴ� ��ġ�� ��ų
/// </summary>
public class Knockback : MonoBehaviour
{
    [SerializeField, Tooltip("���� �ð�")] 
    private float duration = 1.0f;
    private float lifeTime = 0;

    [SerializeField, Tooltip("���� ������")] 
    private ElementDamage damage;

    [SerializeField, Tooltip("�˹� �Ÿ�")] 
    private float knockbackPower = 5f;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        var status = other.GetComponent<CharacterStatus>();
        if (status)
        {
            status.TakeDamage(damage);

            var direction = (other.transform.position - transform.position).normalized;
            var move = other.GetComponent<PlayerMoveRotate>();
            move.Knockback(direction * knockbackPower);
        }
    }
}
