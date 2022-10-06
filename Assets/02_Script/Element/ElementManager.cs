using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ȿ�� �� ���� ������ ����
/// </summary>
public class ElementManager : MonoBehaviour
{
    public static ElementManager Instance { get; private set; }

    private ElementInfo elementInfo;

    [Header("Element Effect Pool")]
    [SerializeField, EnumNamedArray(typeof(ElementType))]
    [Tooltip("���� ȿ�� ������")] 
    private ElementalEffect[] effectPrefabs = new ElementalEffect[(int)ElementType.None];
    
    [SerializeField, Tooltip("Object Pool�� �ʱ� ���� ȿ�� ����")]
    private int initEffectCount = 3;

    private Dictionary<ElementType, Stack<ElementalEffect>> effectPools = new Dictionary<ElementType, Stack<ElementalEffect>>();

    private void Awake()
    {
        Instance = this;

        elementInfo = ScriptableObject.CreateInstance<ElementInfo>();
    }

    private void Start()
    {
        InitPool();
    }

    private void InitPool()
    {
        // ���� �Ӽ���ŭ ������Ʈ Ǯ��
        for (int elementNum = 0; elementNum < (int)ElementType.None; elementNum++)
        {
            var pool = new Stack<ElementalEffect>();
            effectPools.Add((ElementType)elementNum, pool);

            for (int i = 0; i < initEffectCount; i++)
            {
                var effect = Instantiate(effectPrefabs[elementNum]);
                effect.gameObject.SetActive(false);
                pool.Push(effect);
            }
        }
    }

    public ElementalEffect GetEffect(ElementType elementType)
    {
        bool success = effectPools.TryGetValue(elementType, out var pool);
        Debug.Assert(success, $"Error : Undefined Element Effect - {elementType}");

        ElementalEffect effect;
        success = pool.TryPop(out effect);
        if (!success)
        {
            // ����Ʈ �����ؼ� �����Ѵ�
            effect = Instantiate(effectPrefabs[(int)elementType]);
        }
        effect.gameObject.SetActive(true);

        // ����Ʈ ��� ������ �޾ư� �ڵ忡�� �Ѵ�
        return effect;
    }

    public void ReturnEffect(ElementType elementType, ElementalEffect effect)
    {
        bool success = effectPools.TryGetValue(elementType, out var pool);
        Debug.Assert(success, $"Error : Undefined Element Effect - {elementType}");

        pool.Push(effect);
        // ����Ʈ ��� �ʱ�ȭ
        effect.gameObject.SetActive(false);
    }
}
