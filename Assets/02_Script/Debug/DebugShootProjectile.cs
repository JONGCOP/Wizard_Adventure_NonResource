using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���� �׽�Ʈ�� �߻� ����
/// �ۼ��� - ����ö
/// </summary>
public class DebugShootProjectile : MonoBehaviour
{
    public Projectile projectilePrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.Shoot(transform.position, transform.forward);
        }
    }
}
