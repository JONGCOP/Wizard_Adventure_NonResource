using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일정 시간 뒤 시전되는 마법 구현 클래스
/// 작성자 - 차영철
/// </summary>
public class DelayedMagic : Magic
{
    [SerializeField, Tooltip("딜레이 동안 보여줄 효과")]
    private ParticleSystem delayEffectPrefab;

    [SerializeField] 
    private float delayTime = 1.5f;

    [SerializeField, Tooltip("딜레이 후에 발동될 마법")]
    private Magic invokedMagicPrefab;

    public override void StartMagic()
    {
        StartCoroutine(IEInvokeMagic());
    }

    private void Start()
    {
        PoolSystem.Instance.InitPool(delayEffectPrefab, invokedMagicPrefab.PoolSize);
        PoolSystem.Instance.InitPool(invokedMagicPrefab, invokedMagicPrefab.PoolSize);
    }

    private IEnumerator IEInvokeMagic()
    {
        // Pool 생성 전까지 기다리도록 딜레이
        yield return null;

        // 
        var delayEffect = PoolSystem.Instance.GetInstance<ParticleSystem>(delayEffectPrefab);
        delayEffect.transform.position = transform.position;
        delayEffect.transform.rotation = transform.rotation;

        yield return new WaitForSeconds(delayTime);

        var invokedMagic = PoolSystem.Instance.GetInstance<Magic>(invokedMagicPrefab);
        invokedMagic.transform.position = transform.position;
        invokedMagic.transform.rotation = transform.rotation;
        invokedMagic.StartMagic();

        gameObject.SetActive(false);
    }
}
