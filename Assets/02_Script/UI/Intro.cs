using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [SerializeField, Tooltip("화면 가릴 패널")]
    private GameObject panel;
    public VideoPlayer vid;
    public GameObject rawImage;
    [Tooltip("인트로가 끝난 후 플레이어가 이동할 위치")]
    public Transform changePos;

    [SerializeField, Tooltip("해당 지점까지 이동하는데 쓸 시간")]
    private float toMoveTime = 1.5f;

    IEnumerator Start()
    {
        vid.loopPointReached += CheckOver;
        // 타 클래스 초기화를 위한 대기
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