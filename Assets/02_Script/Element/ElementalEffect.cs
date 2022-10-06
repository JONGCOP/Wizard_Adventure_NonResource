using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// 원소 공격으로 인한 효과 VFX 적용 클래스
/// 작성자 - 차영철
/// </summary>
public class ElementalEffect : MonoBehaviour
{
    private ParticleSystem[] effects;
    // 각각 constant, constantMin, constantMax를 저장
    private Vector3[] initSizes;

    private void Awake()
    {
        effects = GetComponentsInChildren<ParticleSystem>();
        
        initSizes = new Vector3[effects.Length];
        for (int i = 0; i < initSizes.Length; i++)
        {
            var startSize = effects[i].main.startSize;
            initSizes[i] = new Vector3(startSize.constant, startSize.constantMin, startSize.constantMax);
        }
    }

    public void SetEffectTarget(Transform target)
    {
        var renderer = target.GetComponent<Renderer>();
        if (!renderer)
        {
            Debug.LogError("Error : There is No Renderer");
            return;
        }

        ParticleSystemShapeType shapeType;
        if (renderer is MeshRenderer)
        {
            shapeType = ParticleSystemShapeType.MeshRenderer;
        }
        else if (renderer is SkinnedMeshRenderer)
        {
            shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        }
        else
        {
            Debug.LogError("Error : Unknown Renderer Type. Can't Matching");
            return;
        }

        foreach (var effect in effects)
        {
            var shape = effect.shape;
            shape.shapeType = shapeType;
            shape.meshRenderer = renderer as MeshRenderer;
            shape.skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
        }
    }

    public void SetEffectSize(int stack)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            var main = effects[i].main;
            var startSize = main.startSize;
            startSize.constant = initSizes[i].x * (1 + (stack - 1) * 0.2f);
            startSize.constantMin = initSizes[i].y * (1 + (stack - 1) * 0.2f);
            startSize.constantMax = initSizes[i].z * (1 + (stack - 1) * 0.2f);
            main.startSize = startSize;
        }
    }
}
