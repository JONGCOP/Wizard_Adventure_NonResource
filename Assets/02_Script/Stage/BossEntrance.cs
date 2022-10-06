using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1. 만약 플레이어가 트리거에 들어가면
//2. Boss Spawn 위치로 위치를 바꾼다.
/// <summary>
/// 보스스테이지 입구에 트리거를 두고 보스 센터로 이동 시키기
/// 효과 추가
/// </summary>
public class BossEntrance : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossSpawn;
    //private Transform bossSpawn;

    [SerializeField] private float playerMoveTime = 3.0f;
    [SerializeField] private Transform playerMovePos;

    [SerializeField] private ParticleSystem teleportEffect;

    [SerializeField] private float startPatternTime = 4.0f;
    [SerializeField] private GameObject bossHp;

    [SerializeField] private Transform bossPortal;

    private bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        bossSpawn = GameObject.Find("Boss Spawn").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.name.Contains("Player") == true)
        {
            print("Trigger Boss Entrance");
            isTriggered = true;
            StartCoroutine(IEBossIntro());
        }
    }

    private IEnumerator IEBossIntro()
    {
        //플레이어 보스 방 진입
        //    -플레이어가 피 채우는 곳까지 이동
        //    (텔레포트 존으로 강제 이동은 보류)
        //-플레이어 조작 막아놓기
        //    -> 플레이어 강제 이동 - 제단 ? 까지
        //플레이어가 제단 도착하면 넉백 시키기
        //    -가능하면 힐팩 있던 곳까지 넉백. 데미지는 안 입힘
        var player = GameManager.player;
        var playerController = player.GetComponent<PlayerController>();
        var playerMoveRotate = player.GetComponent<PlayerMoveRotate>();

        var bossFSM = boss.GetComponent<BossFSM>();
        Debug.Assert(bossFSM, "Error : BossFSM is null");
        //bossAnimator.SetInteger("SkillState", 0);

        playerController.ActiveController(false);
        playerMoveRotate.ToMove(playerMovePos.position, 4);

        yield return new WaitForSeconds(playerMoveTime);
        print("Boss Coming");

        // 보스 등장
        bossFSM.Appearance();
        teleportEffect.Play(true);
        yield return new WaitForSeconds(1);
        boss.transform.position = bossSpawn.position;

        // 패턴 실행
        bossFSM.StartKnockback();

        yield return new WaitForSeconds(startPatternTime);

        BGMPlayer.Instance.Change(BGMPlayer.BGM_Type.Boss);
        BGMPlayer.Instance.VolumeSize = 0.5f;

        bossFSM.GetComponent<CharacterStatus>().onDead += BattleUI.Instance.OnBossBattleEnd;
        bossFSM.StartFSM();
        bossFSM.onDeathFinished += EndStage;
        // 체력 바 보여주기
        BattleUI.Instance.OnBossBattleStart();

        playerController.ActiveController(true);
    }

    /// <summary>
    /// 포탈까지 자동 이동
    /// </summary>
    private void EndStage()
    {
        var player = GameManager.player;
        var playerController = player.GetComponent<PlayerController>();
        var playerMoveRotate = player.GetComponent<PlayerMoveRotate>();

        player.transform.forward = bossPortal.transform.forward * -1;
        playerController.ActiveController(false);
        playerMoveRotate.ToMove(bossPortal.transform.position, 5f);

        BGMPlayer.Instance.Stop();
    }
}
