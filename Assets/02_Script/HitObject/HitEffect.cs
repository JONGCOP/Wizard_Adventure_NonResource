using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �� ���� �ð� �� SetActive(false) ȣ��
/// </summary>
public class HitEffect : MonoBehaviour
{
    [SerializeField, Tooltip("��ƼŬ�� �󸶳� ���ӵǰ� ������")]
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
