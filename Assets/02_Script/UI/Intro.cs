using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [SerializeField, Tooltip("ȭ�� ���� �г�")]
    private GameObject panel;
    public VideoPlayer vid;
    public GameObject rawImage;
    [Tooltip("��Ʈ�ΰ� ���� �� �÷��̾ �̵��� ��ġ")]
    public Transform changePos;

    [SerializeField, Tooltip("�ش� �������� �̵��ϴµ� �� �ð�")]
    private float toMoveTime = 1.5f;

    IEnumerator Start()
    {
        vid.loopPointReached += CheckOver;
        // Ÿ Ŭ���� �ʱ�ȭ�� ���� ���
        yield return null;
        WindowSystem.Instance.OpenWindow(panel, true);
    }

    private void FadeIn()
    {
        Sequence s = DOTween.Sequence();
        s.SetUpdate(true);
        s.Append(GetComponent<CanvasGroup>().DOFade(0, 2f));
        s.onComplete = () =>
        {
            WindowSystem.Instance.CloseWindow(true);
            //GameManager.player.GetComponent<PlayerMoveRotate>().ResetVRRotate();
            PlayerChangePos();
            BGMPlayer.Instance.PlayBGM(BGMPlayer.BGM_Type.NonBattle);
            gameObject.SetActive(false);
        };
    }

    private void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        //print("Video Is Over");
        rawImage.SetActive(false);
        FadeIn();
    }

    private void PlayerChangePos()
    {
        var playerMove = GameManager.player.GetComponent<PlayerMoveRotate>();
        playerMove.ToMove(changePos.transform.position, toMoveTime);
    }

    public void Skip()
    {
        vid.Stop();
        CheckOver(vid);
    }
}