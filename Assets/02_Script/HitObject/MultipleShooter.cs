using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ¿©·¯ °³ÀÇ ProjectileÀ» Èð»Ñ·Á °ø°ÝÇÒ ¼ö ÀÖ°Ô µ½´Â Å¬·¡½º
/// ÀÛ¼ºÀÚ - Â÷¿µÃ¶
/// </summary>
public class MultipleShooter : Magic
{
    [SerializeField] 
    private int shootCount = 5;
    private int remainShootcount;

    [SerializeField]
    private float shootDelay = 0.1f;
    private float timer;

    [SerializeField] 
    private Projectile projectilePrefab;

    private void OnEnable()
    {
        remainShootcount = shootCount;
    }

    private void Start()
    {
        PoolSystem.Instance.InitPool(projectilePrefab, 10);
    }

    private void Update()
    {
        if (timer <= 0.0f)
        {
            if (remainShootcount > 0)
            {
                ShootProjectile(transform.position, transform.forward);
                remainShootcount--;
                timer = shootDelay;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        timer -= Time.deltaTime;
    }

    private void ShootProjectile(Vector3 position, Vector3 direction)
    {
        var projectile = PoolSystem.Instance.GetInstance<Projectile>(projectilePrefab);
        // Èð»Ñ¸®±â
        var newDirection = (direction + UnityEngine.Random.onUnitSphere * 0.2f).normalized;
        projectile.Shoot(position, newDirection);
    }
}
