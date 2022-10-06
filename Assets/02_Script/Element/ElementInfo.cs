using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Fire,
    Ice,
    Lightning,
    None,   // None�� Count ������ �ϱ⵵ ��
}

/// <summary>
/// �Ӽ� ȿ�� ���� Ŭ����
/// �Ӽ��� �ʿ��� ������ Singleton �������� �����Ͽ� ������ ���
/// �ۼ��� - ����ö
/// </summary>
[CreateAssetMenu(fileName = "Element Info", menuName = "Game Data/Element Info")]
public class ElementInfo : ScriptableObject
{

    /// <summary>
    /// �� �Ӽ��� ȭ�� �������� �߰��� ����
    /// ȭ�� ������ - �ֱ������� ������ ������
    /// </summary>
    [Serializable]
    public class FireData
    {
        [SerializeField, Tooltip("�ִ� ����")]
        private int maxStack = 6;
        [SerializeField, Tooltip("���� �ð�")]
        private float duration = 5;
        [SerializeField, Tooltip("�ʱ� ȭ�� ������")]
        private int initDamage = 3;
        [SerializeField, Tooltip("���� �� �߰� ȭ�� ������")]
        private int stackBonusDamage = 2;
        [SerializeField, Tooltip("ȭ�� �� �ʿ� �� �� ������")]
        private float interval = 0.5f;

        // �ܺο��� �б⸸ �����ϵ��� getter ���� ���
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
    /// ���� �Ӽ� ������ ���� ��ġ�� ���� �����ð����� ���ο�(����, �̼�)�� �ش�
    /// </summary>
    [Serializable]
    public class IceData
    {
        [SerializeField, Tooltip("�ִ� ����")]
        private int maxStack = 5;
        [SerializeField, Tooltip("���� �ð�")]
        private float duration = 3.0f;
        [SerializeField, Tooltip("�ʱ� ���ο� ����")]
        private float initSlowPercent = 25f;
        [SerializeField, Tooltip("�߰� ���ο� ����")]
        private float stackBonusSlowPercent = 5f;

        public int MaxStack => maxStack;
        public float Duration => duration;
        public float InitSlowPercent => initSlowPercent;
        public float StackBonusSlowPercent => stackBonusSlowPercent;
    }

    /// <summary>
    /// ���� �Ӽ��� �߰� ���� ���ʽ��� �ش�
    /// </summary>
    [Serializable]
    public class LightningData
    {
        [SerializeField, Tooltip("���� ���ʽ� ���")]
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