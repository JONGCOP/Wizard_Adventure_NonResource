using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 캐릭터(플레이어, 적) 능력치(체력, 공속, 이속 등) 정보
/// 작성자 - 차영철
/// </summary>
[RequireComponent(typeof(Collider))]
public class CharacterStatus : MonoBehaviour
{
    // float는 부동소수점이므로 연산 오차가 발생할 수 있음
    // (1에서 0.1f를 10번 빼도 정확히 0이 되지 않음)
    // 고로 지속시간이 의도치 않게 길어지는 문제를 방지하기 위해 validTime을 정의
    private static readonly float validTime = 0.01f;
    private static readonly float checkTime = 0.1f;
    private static WaitForSeconds ws = new WaitForSeconds(checkTime);
    private static WaitForSeconds slowTime;

    // Event
    public event Action<float> onHpChange;      // 체력 변경
    public event Action onDead;                 // 죽었을 때
    public event Action<float> onSpeedChenge;   // 슬로우
    public event Action onShocked;              // 경직

    [Header("능력치")]
    //[SerializeField, Tooltip("최대 체력")] 
    public int maxHp = 100;
    private int currentHp;
    // 죽은 이후에도 화상 등으로 데미지를 입어 다시 죽는 일을 방지
    private bool isDead = false;

    [SerializeField, Tooltip("경직 판정 기준치")]
    private int shockThreshold = 50;
    [SerializeField, Tooltip("경직 저항도(퍼센트)")]
    private float shockResistPercent = 0;
    [SerializeField, Tooltip("경직 회복량(초 기준)")]
    private float shockRecovery = 15f;
    private float currentShockAmount = 0;

    [SerializeField, Tooltip("속도 계수(공속, 이속)")] 
    private float speedMultiplier = 1.0f;

    [Header("원소 효과")] 
    [SerializeField, Tooltip("이펙트 대상이 될 트랜스폼")]
    private Transform effectTarget;

    // 각 지속효과 별 스택
    private int burnStack = 0;  // 화상
    private int slowStack = 0;  // 슬로우

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

    // 패턴에 따라 경직 저항도가 강해지거나 약해질 수 있도록 설정 가능
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
                // 스택 단계에 따른 효과 조정
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

            // 슬로우 해제됨
            if (slowStack == 0)
            {
                speedMultiplier = 1;
                ReturnElementEffect(ElementType.Ice);
            }
            // 슬로우 걸림
            else
            {
                // 스택 단계에 따른 효과 조정
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
        // 경직 회복
        currentShockAmount = math.max(currentShockAmount - shockRecovery * Time.deltaTime, 0);
    }

    private void OnDisable()
    {
        // 죽었을 때 원소 효과 반환
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

        // 불 리셋
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        burnCoroutine = null;
        BurnStack = 0;

        // 얼음 리셋
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = null;
        SlowStack = 0;

        // 전기 리셋
        ReturnElementEffect(ElementType.Lightning);
    }

    /// <summary>
    /// 데미지 및 속성 적용
    /// </summary>
    public void TakeDamage(ElementDamage elementDamage)
    {
        // 데미지 정보 적용
        CurrentHp -= elementDamage.damage;
        AddShock(elementDamage.damage);

        // 속성에 따라 이펙트 적용
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
                // 화상 데미지 입히기
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
        
        // 슬로우 스택 리셋
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

            // 이펙트를 몸체에 달기
            elementEffects[(int)elementType].SetEffectTarget(effectTarget);

        }

        return elementEffects[(int)elementType];
    }

    private void ReturnElementEffect(ElementType elementType)
    {
        if (!elementEffects[(int)elementType])
        {
            // null일 땐 Object Pool에 effect 반환 안 함
            return;
        }

        ElementManager.Instance.ReturnEffect(elementType, elementEffects[(int)elementType]);
        elementEffects[(int)elementType] = null;
    }
}
