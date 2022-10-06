using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 엔딩 재생 및 타이틀로 돌아가는 기능
/// 작성자 - 차영철
/// </summary>
public class Outro : MonoBehaviour
{

    [SerializeField, Tooltip("엔딩 영상 출력 비디오")]
    private VideoPlayer endingPlayer;
    [SerializeField, Tooltip("영상 출력 받는 이미지")]
    private GameObject rawImage;
    [SerializeField] 
    private GameObject fadeInPanel;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = fadeInPanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        WindowSystem.Instance.OpenWindow(fadeInPanel, true);

        canvasGroup.alpha = 0f;
        Sequence s = DOTween.Sequence();
        s.SetUpdate(true);
        s.Append(canvasGroup.DOFade(1, 2.0f));
        s.onComplete = () => {
            endingPlayer.Play();
        };

        endingPlayer.loopPointReached += ReturnTitle;
        var controller = GameManager.Controller;
        // 게임이 끝났으면 조작 불가능
        controller.ActiveController(false);
    }

    private void ReturnTitle(VideoPlayer vp)
    {
        SceneManager.LoadScene(0);
    }
}
