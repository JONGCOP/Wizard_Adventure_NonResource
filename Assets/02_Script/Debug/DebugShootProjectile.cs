using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 공격 테스트용 발사 더미
/// 작성자 - 차영철
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
