using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마법을 쏘는 행동(손을 뻗음)을 했는지 감지
/// 작성자 - 차영철
/// </summary>
public class ShootDetector : MonoBehaviour
{
    [SerializeField, Tooltip("플레이어 조작 클래스")]
    private PlayerController playerController;

    [SerializeField, Tooltip("발사할 기본 마법 정보를 담은 클래스")] 
    private PlayerMagic playerMagic;

    [SerializeField, Tooltip("오른손 Transform")]
    private Transform rightHandTr;

    private float reviseAngle = 15f;
    private float reviseAngleCos;

    private void Start()
    {
        reviseAngleCos = Mathf.Cos(Mathf.Deg2Rad * reviseAngle);
        print($"revise Angle : {reviseAngleCos}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerController.CanControlPlayer ||
            !playerController.CheckLearnedAbility(PlayerController.MagicAbility.Base))
        {
            return;
        }

        // 충돌 감지 - 유저가 바라보는 방향으로 손을 뻗었을 때 충돌 판정한다
        if (other.CompareTag("Right Hand"))
        {
            // 손의 각도가 기울어지는 경우 마법의 명중률이 떨어진다
            // 고로 발사 각도를 보정한다
            var latestRoom = GameManager.Instance.LatestRoom;
            if (playerMagic.CurrentElement == ElementType.Lightning || latestRoom == null)
            {
                playerMagic.ShootMagic(other.transform.position, rightHandTr.forward);
                return;
            }

            var targets = latestRoom.Targets;
            if (targets == null || targets.Length < 1)
            {
                playerMagic.ShootMagic(other.transform.position, rightHandTr.forward);
                return;
            }

            float minAngleCos = -1;
            Vector3 nearestDir = Vector3.one;
            foreach (var status in targets)
            {
                var target = status.transform;
                var targetPos = target.position;
                var collider = target.GetComponent<Collider>();
                if (collider is CapsuleCollider)
                {
                    targetPos += ((CapsuleCollider)collider).center;
                }
                else if (collider is BoxCollider)
                {
                    targetPos += ((BoxCollider)collider).center;
                }
                else if (collider is SphereCollider)
                {
                    targetPos += ((SphereCollider)collider).center;
                }
                var dir = (targetPos - rightHandTr.position).normalized;
                var angleCos = Vector3.Dot(dir, rightHandTr.forward);
                if (angleCos > minAngleCos)
                {
                    minAngleCos = angleCos;
                    nearestDir = dir;
                }
            }

            if (minAngleCos > reviseAngleCos)
            {
                playerMagic.ShootMagic(rightHandTr.position, nearestDir);
            }
            else
            {
                playerMagic.ShootMagic(rightHandTr.position, rightHandTr.forward);
            }
        }
    }
}
