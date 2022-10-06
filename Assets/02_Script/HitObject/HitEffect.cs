using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맞췄을 때 일정 시간 뒤 SetActive(false) 호출
/// </summary>
public class HitEffect : MonoBehaviour
{
    [SerializeField, Tooltip("파티클이 얼마나 지속되고 꺼질지")]
    private float duration = 2.4f;

    private void OnEnable()
    {
        Invoke(nameof(TurnOff), duration);
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
