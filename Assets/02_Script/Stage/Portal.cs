using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


//�޸���
//�ش� ��ġ�� �÷��̾� �̵�
//���� Ʃ�丮�� �������� �� Ŭ���� ���¶��
//�ش� ��ġ�� �̵� �Ұ�

//�ڷ�ƾ
//ȭ�� ���̵�ƿ�
//�ش� ��ġ�� �÷��̾� �̵�
//ȭ�� ���̵� ��
/// <summary>
/// �� �������� �� ���� �� �� ���� ���� ��Ż Ȱ��ȭ ����
/// �÷��̾� ��Ż�̵�
/// �ۼ��� - ���ؼ�
/// 0912 �߰� : audioSource �� ��Ż Ż�� fadeInOut UI����
/// </summary>
public class Portal : MonoBehaviour
{
    protected GameObject player;
    [SerializeField, Tooltip("�濡 �ִ� ���͵�")]
    protected CharacterStatus[] targets;
    protected Vector3[] initPoses;
    protected int remainCount;    //���� �� ��
    protected bool canUse = false;     //Ʈ���� ����ġ

    [Header("��Ż / �÷��̾� ���� ��ġ")]
    public GameObject portal;
    protected Collider portalCol;
    public RoomStartPoint portalPoint;   //��Ż ź �� �÷��̾� ���� ��ġ

    [SerializeField, Tooltip("��Ż ���� �� ����")]
    private AudioClip openSound;
    [SerializeField, Tooltip("��Ż Ż �� ����")]
    private AudioClip portalSound;
    [SerializeField, Tooltip("���� �� ����� ���")]
    private BGMPlayer.BGM_Type bgmType = BGMPlayer.BGM_Type.TutoBattle;

    private AudioSource audioSource;

    [SerializeField, Tooltip("Ȱ��ȭ�� ���� ��")]
    private GameObject nextRoom;
    [SerializeField, Tooltip("��Ȱ��ȭ�� ���� ��")]
    private GameObject currentRoom;

    [Tooltip("��Ż Ż �� fade In Out ����")]
    public GameObject fadeImage;
    public CanvasGroup canvasGroup; //���̵� �ξƿ� ĵ����

    public CharacterStatus[] Targets => targets;

    private void Awake()
    {
        initPoses = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            initPoses[i] = targets[i].transform.position;
        }

        //��Ż ����
        audioSource = GetComponent<AudioSource>();
        portalCol = GetComponent<BoxCollider>();
        portalCol.enabled = false;
        canUse = false;
        portal = transform.GetChild(0).gameObject;
        portal.SetActive(false);

        //���̵��ξƿ� ������Ʈ �ʱ� ��Ȱ��ȭ
        fadeImage.SetActive(false);

        var room = transform.parent;
        currentRoom = room.gameObject;

        var stage = room.parent;
        int sibilingIndex = room.GetSiblingIndex();
        if (sibilingIndex < stage.childCount - 1)
        {
            nextRoom = stage.GetChild(sibilingIndex + 1).gameObject;
        }
    }

    protected void Start()
    {
        player = GameManager.player;
    }

    /// <summary>
    /// ���� �ʱ�ȭ�Ѵ�
    /// </summary>
    public virtual void ResetRoom()
    {
        remainCount = targets.Length;
        BattleUI.Instance.SetRemainCount(remainCount, targets.Length);
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].gameObject.SetActive(true);

            // ���� ���� ����
            targets[i].ResetStatus();
            var fsm = targets[i].GetComponent<EnemyFSM>();
            if (fsm)
            {
                fsm.ResetFSM();
            }
            
            // ���� ��ġ ����
            targets[i].transform.position = initPoses[i];
        }

        // �÷��̾�� ���Ͱ� ���� �������� �� ��Ż ����
        // ���� ��Ż�� �����ؾ��Ѵ�
    }

    // ��Ż ����
    public virtual void StartRoom()
    {
        if (targets.Length > 0)
        {
            BGMPlayer.Instance.Change(bgmType);
        }

        foreach (var target in targets)
        {
            target.gameObject.SetActive(true);
            target.onDead += DecreaseCount;
        }
        // ���� ���� ����
        remainCount = targets.Length;
        BattleUI.Instance.SetRemainCount(remainCount, targets.Length);
        CheckEnd();
    }

    protected void DecreaseCount()
    {
        remainCount--;
        BattleUI.Instance.SetRemainCount(remainCount, targets.Length);
        CheckEnd();
    }

    protected void CheckEnd()
    {
        if (remainCount <= 0)
        {
            canUse = true;
            portalCol.enabled = true;
            // ��Ż Ȱ��ȭ
            portal.SetActive(true);
            audioSource.PlayOneShot(openSound);
            BGMPlayer.Instance.Rollback();
        }
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        if (canUse)
        {
            // ��Ż �ߺ� ��� �� �ǰ� ����
            canUse = false;
            OnUsePortal();
        }
    }

    /// <summary>
    /// ���� ��Ż ��� ��� (����׿�)
    /// </summary>
    public void UsePortal()
    {
        print("Debug : Forced Use Portal");
        OnUsePortal();
    }

    protected virtual void OnUsePortal()
    {
        StartCoroutine(IEUsePortal());
    }

    //���̵� �ξƿ� ���� �Լ�
    private void Fade()
    {
        fadeImage.SetActive(true);

        DOTween.Sequence()
            .Append(canvasGroup.DOFade(1.0f, 3f))   //���� ����� ���̵�ƿ�
            .Append(canvasGroup.DOFade(0.0f, 3f))   //���� ����� ���̵���
            .OnComplete(() =>                       //�Ϸ� �Ŀ��� canvasgroup, Canvas ������Ʈ ��Ȱ��ȭ
            {
                fadeImage.SetActive(false);
            });
    }

    private IEnumerator IEUsePortal()
    {
        //��Ż ���� �۵� - �ۼ��� ���ؼ�
        audioSource.PlayOneShot(portalSound);
        yield return new WaitForSeconds(0.1f);
        Fade();
        yield return new WaitForSeconds(2.5f);
        // ��Ż �̵� ����
        nextRoom.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // �� �̵�
        player.GetComponent<PlayerMoveRotate>().SetPos(portalPoint.transform.position, portalPoint.transform.forward);

        // ��Ż Ÿ�� �� ���� Fade Out �Ǹ鼭 ���� �� �Ѿ�� ����
        yield return new WaitForSeconds(3.0f);
        currentRoom.SetActive(false);
    }
}