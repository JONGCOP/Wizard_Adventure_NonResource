using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//플레이어 스폰 트리거 되면
//적 공장에서 적 생성
//적 소서러, 궁수, 근접 랜덤 생성
//적 수

/// <summary>
/// 텔레포트 되면 적 활성화
/// </summary>
public class EnemySpawn : RoomStartPoint
{
    [SerializeField, Tooltip("방 이동했을 때 띄울 튜토리얼 창 정보")]
    private TutorialExplainData[] tutorialDatas;
    [SerializeField, Tooltip("배울 기능")]
    private PlayerController.MagicAbility[] learnAbilities;

    protected override void OnTrigger()
    {
        base.OnTrigger();

        // 튜토리얼 활성화
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