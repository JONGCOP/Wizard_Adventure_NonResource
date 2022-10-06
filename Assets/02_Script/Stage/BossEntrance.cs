using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1. ���� �÷��̾ Ʈ���ſ� ����
//2. Boss Spawn ��ġ�� ��ġ�� �ٲ۴�.
/// <summary>
/// ������������ �Ա��� Ʈ���Ÿ� �ΰ� ���� ���ͷ� �̵� ��Ű��
/// ȿ�� �߰�
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
        //�÷��̾� ���� �� ����
        //    -�÷��̾ �� ä��� ������ �̵�
        //    (�ڷ���Ʈ ������ ���� �̵��� ����)
        //-�÷��̾� ���� ���Ƴ���
        //    -> �÷��̾� ���� �̵� - ���� ? ����
        //�÷��̾ ���� �����ϸ� �˹� ��Ű��
        //    -�����ϸ� ���� �ִ� ������ �˹�. �������� �� ����
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

        // ���� ����
        bossFSM.Appearance();
        teleportEffect.Play(true);
        yield return new WaitForSeconds(1);
        boss.transform.position = bossSpawn.position;

        // ���� ����
        bossFSM.StartKnockback();

        yield return new WaitForSeconds(startPatternTime);

        BGMPlayer.Instance.Change(BGMPlayer.BGM_Type.Boss);
        BGMPlayer.Instance.VolumeSize = 0.5f;

        bossFSM.GetComponent<CharacterStatus>().onDead += BattleUI.Instance.OnBossBattleEnd;
        bossFSM.StartFSM();
        bossFSM.onDeathFinished += EndStage;
        // ü�� �� �����ֱ�
        BattleUI.Instance.OnBossBattleStart();

        playerController.ActiveController(true);
    }

    /// <summary>
    /// ��Ż���� �ڵ� �̵�
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
