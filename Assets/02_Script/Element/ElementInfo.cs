using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Fire,
    Ice,
    Lightning,
    None,   // None은 Count 역할을 하기도 함
}

/// <summary>
/// 속성 효과 정의 클래스
/// 속성에 필요한 정보를 Singleton 형식으로 정의하여 가져다 사용
/// 작성자 - 차영철
/// </summary>
[CreateAssetMenu(fileName = "Element Info", menuName = "Game Data/Element Info")]
public class ElementInfo : ScriptableObject
{

    /// <summary>
    /// 불 속성은 화상 데미지를 추가로 입힘
    /// 화상 데미지 - 주기적으로 데미지 입히기
    /// </summary>
    [Serializable]
    public class FireData
    {
        [SerializeField, Tooltip("최대 스택")]
        private int maxStack = 6;
        [SerializeField, Tooltip("지속 시간")]
        private float duration = 5;
        [SerializeField, Tooltip("초기 화상 데미지")]
        private int initDamage = 3;
        [SerializeField, Tooltip("스택 당 추가 화상 데미지")]
        private int stackBonusDamage = 2;
        [SerializeField, Tooltip("화상 몇 초에 한 번 입힐지")]
        private float interval = 0.5f;

        // 외부에서 읽기만 가능하도록 getter 만들어서 사용
        public int MaxStack => maxStack;
        public float Duration => duration;
        public int InitDamage => initDamage;
        public int StackBonusDamage => stackBonusDamage;
        public float Interval => interval;

        public int BurnDamage(int stack)
        {
            return initDamage + (stack - 1) * stackBonusDamage;
        }
    }

    /// <summary>
    /// 얼음 속성 공격은 빙결 수치에 따라 일정시간동안 슬로우(공속, 이속)를 준다
    /// </summary>
    [Serializable]
    public class IceData
    {
        [SerializeField, Tooltip("최대 스택")]
        private int maxStack = 5;
        [SerializeField, Tooltip("지속 시간")]
        private float duration = 3.0f;
        [SerializeField, Tooltip("초기 슬로우 정도")]
        private float initSlowPercent = 25f;
        [SerializeField, Tooltip("추가 슬로우 정도")]
        private float stackBonusSlowPercent = 5f;

        public int MaxStack => maxStack;
        public float Duration => duration;
        public float InitSlowPercent => initSlowPercent;
        public float StackBonusSlowPercent => stackBonusSlowPercent;
    }

    /// <summary>
    /// 전기 속성은 추가 경직 보너스를 준다
    /// </summary>
    [Serializable]
    public class LightningData
    {
        [SerializeField, Tooltip("경직 보너스 배수")]
        private float addedShockMultiplier = 2.5f;

        public float AddedShockMultiplier => addedShockMultiplier;
    }
    
    public static FireData Fire { get; private set; }
    public static IceData Ice { get; private set; }
    public static LightningData Lightning { get; private set; }

    [SerializeField] 
    private FireData fireData = new();
    [SerializeField]
    private IceData iceData = new();
    [SerializeField]
    private LightningData lightningData = new();

    private void Awake()
    {
        Debug.Assert(fireData != null, "Error : Fire Data is null");
        Debug.Assert(iceData != null, "Error : Ice Data is null");
        Debug.Assert(lightningData != null, "Error : Lightning Data is null");

        Fire = fireData;
        Ice = iceData;
        Lightning = lightningData;
        Debug.Log("Element Info Init");
    }
}