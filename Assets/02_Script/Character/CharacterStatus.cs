using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ĳ����(�÷��̾�, ��) �ɷ�ġ(ü��, ����, �̼� ��) ����
/// �ۼ��� - ����ö
/// </summary>
[RequireComponent(typeof(Collider))]
public class CharacterStatus : MonoBehaviour
{
    // float�� �ε��Ҽ����̹Ƿ� ���� ������ �߻��� �� ����
    // (1���� 0.1f�� 10�� ���� ��Ȯ�� 0�� ���� ����)
    // ��� ���ӽð��� �ǵ�ġ �ʰ� ������� ������ �����ϱ� ���� validTime�� ����
    private static readonly float validTime = 0.01f;
    private static readonly float checkTime = 0.1f;
    private static WaitForSeconds ws = new WaitForSeconds(checkTime);
    private static WaitForSeconds slowTime;

    // Event
    public event Action<float> onHpChange;      // ü�� ����
    public event Action onDead;                 // �׾��� ��
    public event Action<float> onSpeedChenge;   // ���ο�
    public event Action onShocked;              // ����

    [Header("�ɷ�ġ")]
    //[SerializeField, Tooltip("�ִ� ü��")] 
    public int maxHp = 100;
    private int currentHp;
    // ���� ���Ŀ��� ȭ�� ������ �������� �Ծ� �ٽ� �״� ���� ����
    private bool isDead = false;

    [SerializeField, Tooltip("���� ���� ����ġ")]
    private int shockThreshold = 50;
    [SerializeField, Tooltip("���� ���׵�(�ۼ�Ʈ)")]
    private float shockResistPercent = 0;
    [SerializeField, Tooltip("���� ȸ����(�� ����)")]
    private float shockRecovery = 15f;
    private float currentShockAmount = 0;

    [SerializeField, Tooltip("�ӵ� ���(����, �̼�)")] 
    private float speedMultiplier = 1.0f;

    [Header("���� ȿ��")] 
    [SerializeField, Tooltip("����Ʈ ����� �� Ʈ������")]
    private Transform effectTarget;

    // �� ����ȿ�� �� ����
    private int burnStack = 0;  // ȭ��
    private int slowStack = 0;  // ���ο�

    private Coroutine burnCoroutine;
    private Coroutine slowCoroutine;

    private ElementalEffect[] elementEffects = new ElementalEffect[(int)ElementType.None];

    private Collider hitCollider;

    #region Properties
    public int CurrentHp
    {
        get => currentHp;
        private set
        {
            if (isDead)
            {
                return;
            }

            currentHp = value;
            onHpChange?.Invoke((float)currentHp / maxHp);
            if (currentHp <= 0)
            {
                currentHp = 0;
                isDead = true;
                onDead?.Invoke();
                hitCollider.enabled = false;
            }
        }
    }

    // ���Ͽ� ���� ���� ���׵��� �������ų� ������ �� �ֵ��� ���� ����
    public float ShockResistPercent
    {
        get => shockResistPercent;
        set => shockResistPercent = value;
    }

    public int BurnStack
    {
        get => burnStack;
        private set
        {
            var fireInfo = ElementInfo.Fire;
            burnStack = math.min(value, fireInfo.MaxStack);

            if (burnStack == 0)
            {
                ReturnElementEffect(ElementType.Fire);
            }
            else
            {
                // ���� �ܰ迡 ���� ȿ�� ����
                var effect = GetElementEffect(ElementType.Fire);
                effect.SetEffectSize(burnStack);
            }
        }
    }

    public int SlowStack
    {
        get => slowStack;
        private set
        {
            var iceInfo = ElementInfo.Ice;
            slowStack = math.min(value, iceInfo.MaxStack);

            // ���ο� ������
            if (slowStack == 0)
            {
                speedMultiplier = 1;
                ReturnElementEffect(ElementType.Ice);
            }
            // ���ο� �ɸ�
            else
            {
                // ���� �ܰ迡 ���� ȿ�� ����
                speedMultiplier = 1 - (iceInfo.InitSlowPercent + iceInfo.StackBonusSlowPercent * slowStack) * 0.01f;
                var effect = GetElementEffect(ElementType.Ice);
                effect.SetEffectSize(slowStack);
            }
            onSpeedChenge?.Invoke(speedMultiplier);
        }
    }
    #endregion


    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        ResetStatus();
    }

    private void Start()
    {
        slowTime = new WaitForSeconds(ElementInfo.Ice.Duration);
    }

    private void Update()
    {
        // ���� ȸ��
        currentShockAmount = math.max(currentShockAmount - shockRecovery * Time.deltaTime, 0);
    }

    private void OnDisable()
    {
        // �׾��� �� ���� ȿ�� ��ȯ
        for (int elementNum = 0; elementNum < (int)ElementType.None; elementNum++)
        {
            ReturnElementEffect((ElementType)elementNum);
        }
    }

    public void ResetStatus()
    {
        isDead = false;
        CurrentHp = maxHp;
        hitCollider.enabled = true;

        // �� ����
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        burnCoroutine = null;
        BurnStack = 0;

        // ���� ����
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = null;
        SlowStack = 0;

        // ���� ����
        ReturnElementEffect(ElementType.Lightning);
    }

    /// <summary>
    /// ������ �� �Ӽ� ����
    /// </summary>
    public void TakeDamage(ElementDamage elementDamage)
    {
        // ������ ���� ����
        CurrentHp -= elementDamage.damage;
        AddShock(elementDamage.damage);

        // �Ӽ��� ���� ����Ʈ ����
        switch (elementDamage.elementType)
        {
            case ElementType.Fire:
                if (burnCoroutine != null)
                {
                    StopCoroutine(burnCoroutine);
                }
                burnCoroutine = StartCoroutine(IEApplyBurn(elementDamage.stack));
                break;
            case ElementType.Ice:
                if (slowCoroutine != null)
                {
                    StopCoroutine(slowCoroutine);
                }
                slowCoroutine = StartCoroutine(IESlow(elementDamage.stack));
                break;
            case ElementType.Lightning:
                AddShock(elementDamage.damage * (ElementInfo.Lightning.AddedShockMultiplier));
                StopCoroutine(nameof(IEElectricShockEffect));
                StartCoroutine(nameof(IEElectricShockEffect));
                break;
        }
    }

    private void AddShock(float addedAmount)
    {
        currentShockAmount += addedAmount * (1 - 0.01f * shockResistPercent);

        if (currentShockAmount > shockThreshold)
        {
            onShocked?.Invoke();
            currentShockAmount = 0;
        }
        // 
        else if (currentShockAmount < 0)
        {
            currentShockAmount = 0;
        }
    }

    private IEnumerator IEApplyBurn(int addedStack)
    {
        BurnStack = burnStack + addedStack;

        var fireInfo = ElementInfo.Fire;
        float remainTime = fireInfo.Duration;
        float interval = fireInfo.Interval;
        while (remainTime > validTime)
        {
            remainTime -= checkTime;
            interval -= checkTime;

            if (interval < validTime)
            {
                // ȭ�� ������ ������
                CurrentHp -= fireInfo.InitDamage + fireInfo.StackBonusDamage * burnStack;
                interval = fireInfo.Interval;
            }

            yield return ws;
        }

        BurnStack = 0;
        burnCoroutine = null;
    }

    private IEnumerator IESlow(int addedStack)
    {
        SlowStack += addedStack;
        yield return slowTime;
        
        // ���ο� ���� ����
        SlowStack = 0;
        slowCoroutine = null;
    }

    private IEnumerator IEElectricShockEffect()
    {
        var effect = GetElementEffect(ElementType.Lightning);
        yield return new WaitForSeconds(0.3f);
        ReturnElementEffect(ElementType.Lightning);
    }

    private ElementalEffect GetElementEffect(ElementType elementType)
    {
        //ref var effect = elementEffects[(int)elementType];
        if (!elementEffects[(int)elementType])
        {
            elementEffects[(int)elementType] = ElementManager.Instance.GetEffect(elementType);

            // ����Ʈ�� ��ü�� �ޱ�
            elementEffects[(int)elementType].SetEffectTarget(effectTarget);

        }

        return elementEffects[(int)elementType];
    }

    private void ReturnElementEffect(ElementType elementType)
    {
        if (!elementEffects[(int)elementType])
        {
            // null�� �� Object Pool�� effect ��ȯ �� ��
            return;
        }

        ElementManager.Instance.ReturnEffect(elementType, elementEffects[(int)elementType]);
        elementEffects[(int)elementType] = null;
    }
}
