using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��Ż ��ũ��Ʈ
/// �ۼ��� - ����ö
/// </summary>
public class BossRoomPortal : Portal
{
    [Header("Outro")]
    [SerializeField, Tooltip("�ƿ�Ʈ�� ����")] 
    private GameObject outro;

    [SerializeField, Tooltip("���� �缳�� ��ġ")]
    private Transform bossSpawn;

    /// <summary>
    /// ���� �ʱ�ȭ�Ѵ�
    /// </summary>
    public override void ResetRoom()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].gameObject.SetActive(true);

            // ���� ���� ����
            targets[i].ResetStatus();

            // ���� ��ġ ����
            targets[i].transform.position = bossSpawn.position;
        }

        // �÷��̾�� ���Ͱ� ���� �������� �� ��Ż ����
        // ���� ��Ż�� �����ؾ��Ѵ�
    }

    // ��Ż ����
    public override void StartRoom()
    {
        foreach (var target in targets)
        {
            target.gameObject.SetActive(true);
            target.onDead += OpenPortal;
        }
    }

    private void OpenPortal()
    {
        canUse = true;
        portalCol.enabled = true;
        // ��Ż Ȱ��ȭ
        portal.SetActive(true);
    }

    protected override void OnUsePortal()
    {
        // �ƿ�Ʈ�� Ʋ��
        outro.SetActive(true);
    }
}
