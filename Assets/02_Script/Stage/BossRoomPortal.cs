using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 포탈 스크립트
/// 작성자 - 차영철
/// </summary>
public class BossRoomPortal : Portal
{
    [Header("Outro")]
    [SerializeField, Tooltip("아웃트로 영상")] 
    private GameObject outro;

    [SerializeField, Tooltip("보스 재설정 위치")]
    private Transform bossSpawn;

    /// <summary>
    /// 방을 초기화한다
    /// </summary>
    public override void ResetRoom()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].gameObject.SetActive(true);

            // 몬스터 상태 리셋
            targets[i].ResetStatus();

            // 몬스터 위치 리셋
            targets[i].transform.position = bossSpawn.position;
        }

        // 플레이어와 몬스터가 같이 전멸했을 때 포탈 열림
        // 열린 포탈도 리셋해야한다
    }

    // 포탈 시작
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
        // 포탈 활성화
        portal.SetActive(true);
    }

    protected override void OnUsePortal()
    {
        // 아웃트로 틀기
        outro.SetActive(true);
    }
}
