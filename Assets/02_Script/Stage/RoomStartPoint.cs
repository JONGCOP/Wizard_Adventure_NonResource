using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방에 진입하면 시작하는 기능
/// </summary>
public class RoomStartPoint : MonoBehaviour
{
    [SerializeField, Tooltip("활성화할 포탈")]
    protected Portal portal;
    protected bool isTriggered = false;

    [SerializeField, Tooltip("지정할 체크포인트\n" +
                             "지정하지 않으면 RoomStartPoint를 기준으로 삼음")]
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
