using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 쏜 투사체 공격을 감지하여 TimeScale을 조정하는 클래스
/// </summary>
public class ProjectileDetector : MonoBehaviour
{
    [SerializeField, Tooltip("적 투사체가 들어왔을 때 느려지는 시간")] 
    private float slowTimeScale = 0.5f;

    // 현재 판정 범위 내에 들어와있는 투사체
    private int remainProjectileCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // 투사체인지 확인하고 막기
        if (other.CompareTag("Projectile"))
        {
            remainProjectileCount++;
            Debug.Assert(remainProjectileCount > 0, "Error : remain Projectile Count can't lower than 0");
            Time.timeScale = slowTimeScale;
            other.GetComponent<Projectile>().onDestroy += DestroyProjectile;
        }
    }

    // 주의 : 충돌로 도중 사라진 경우 Trigger Exit가 호출되지 않음
    private void OnTriggerExit(Collider other)
    {
        // 판정선 밖으로 나갔을 때
        if (other.CompareTag("Projectile"))
        {
            DestroyProjectile();
            // 감지할 필요가 없는 투사체는 감지 대상에서 벗어난다
            other.GetComponent<Projectile>().onDestroy -= DestroyProjectile;
        }
    }

    private void DestroyProjectile()
    {
        remainProjectileCount--;
        if (remainProjectileCount == 0)
        {
            Time.timeScale = 1.0f;
        }
    }
}
