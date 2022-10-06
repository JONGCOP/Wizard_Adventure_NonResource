using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �濡 �����ϸ� �����ϴ� ���
/// </summary>
public class RoomStartPoint : MonoBehaviour
{
    [SerializeField, Tooltip("Ȱ��ȭ�� ��Ż")]
    protected Portal portal;
    protected bool isTriggered = false;

    [SerializeField, Tooltip("������ üũ����Ʈ\n" +
                             "�������� ������ RoomStartPoint�� �������� ����")]
    protected Transform checkPoint;

    private void Awake()
    {
        if (!checkPoint)
        {
            checkPoint = transform;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.name.Contains("Player"))
        {
            OnTrigger();
        }
    }

    protected virtual void OnTrigger()
    {
        portal.StartRoom();
        isTriggered = true;

        GameManager.Instance.SetCheckPoint(checkPoint, portal);
    }
}
