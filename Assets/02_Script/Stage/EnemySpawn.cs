using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//�÷��̾� ���� Ʈ���� �Ǹ�
//�� ���忡�� �� ����
//�� �Ҽ���, �ü�, ���� ���� ����
//�� ��

/// <summary>
/// �ڷ���Ʈ �Ǹ� �� Ȱ��ȭ
/// </summary>
public class EnemySpawn : RoomStartPoint
{
    [SerializeField, Tooltip("�� �̵����� �� ��� Ʃ�丮�� â ����")]
    private TutorialExplainData[] tutorialDatas;
    [SerializeField, Tooltip("��� ���")]
    private PlayerController.MagicAbility[] learnAbilities;

    protected override void OnTrigger()
    {
        base.OnTrigger();

        // Ʃ�丮�� Ȱ��ȭ
        if (tutorialDatas != null && tutorialDatas.Length != 0)
        {
            var window = WindowSystem.tutorialWindow;
            WindowSystem.Instance.OpenWindow(window.gameObject, true);
            window.Open(tutorialDatas);

            var playerController = GameManager.player.GetComponent<PlayerController>();
            for (int i = 0; i < learnAbilities.Length; i++)
            {
                playerController.LearnAbility(learnAbilities[i]);
            }
        }
    }
}