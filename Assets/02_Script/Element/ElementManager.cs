using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 원소 효과 및 원소 데이터 저장
/// </summary>
public class ElementManager : MonoBehaviour
{
    public static ElementManager Instance { get; private set; }

    private ElementInfo elementInfo;

    [Header("Element Effect Pool")]
    [SerializeField, EnumNamedArray(typeof(ElementType))]
    [Tooltip("원소 효과 프리팹")] 
    private ElementalEffect[] effectPrefabs = new ElementalEffect[(int)ElementType.None];
    
    [SerializeField, Tooltip("Object Pool할 초기 원소 효과 개수")]
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
        // 원소 속성만큼 오브젝트 풀링
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
            // 이펙트 생성해서 리턴한다
            effect = Instantiate(effectPrefabs[(int)elementType]);
        }
        effect.gameObject.SetActive(true);

        // 이펙트 대상 설정은 받아간 코드에서 한다
        return effect;
    }

    public void ReturnEffect(ElementType elementType, ElementalEffect effect)
    {
        bool success = effectPools.TryGetValue(elementType, out var pool);
        Debug.Assert(success, $"Error : Undefined Element Effect - {elementType}");

        pool.Push(effect);
        // 이펙트 대상 초기화
        effect.gameObject.SetActive(false);
    }
}
