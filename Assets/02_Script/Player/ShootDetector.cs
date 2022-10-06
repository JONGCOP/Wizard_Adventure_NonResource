using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��� �ൿ(���� ����)�� �ߴ��� ����
/// �ۼ��� - ����ö
/// </summary>
public class ShootDetector : MonoBehaviour
{
    [SerializeField, Tooltip("�÷��̾� ���� Ŭ����")]
    private PlayerController playerController;

    [SerializeField, Tooltip("�߻��� �⺻ ���� ������ ���� Ŭ����")] 
    private PlayerMagic playerMagic;

    [SerializeField, Tooltip("������ Transform")]
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

        // �浹 ���� - ������ �ٶ󺸴� �������� ���� ������ �� �浹 �����Ѵ�
        if (other.CompareTag("Right Hand"))
        {
            // ���� ������ �������� ��� ������ ���߷��� ��������
            // ��� �߻� ������ �����Ѵ�
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
