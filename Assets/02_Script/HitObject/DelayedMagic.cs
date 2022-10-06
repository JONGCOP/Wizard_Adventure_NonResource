using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �ð� �� �����Ǵ� ���� ���� Ŭ����
/// �ۼ��� - ����ö
/// </summary>
public class DelayedMagic : Magic
{
    [SerializeField, Tooltip("������ ���� ������ ȿ��")]
    private ParticleSystem delayEffectPrefab;

    [SerializeField] 
    private float delayTime = 1.5f;

    [SerializeField, Tooltip("������ �Ŀ� �ߵ��� ����")]
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
        // Pool ���� ������ ��ٸ����� ������
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
