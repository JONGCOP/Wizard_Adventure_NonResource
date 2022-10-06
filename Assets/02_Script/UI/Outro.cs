using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���� ��� �� Ÿ��Ʋ�� ���ư��� ���
/// �ۼ��� - ����ö
/// </summary>
public class Outro : MonoBehaviour
{

    [SerializeField, Tooltip("���� ���� ��� ����")]
    private VideoPlayer endingPlayer;
    [SerializeField, Tooltip("���� ��� �޴� �̹���")]
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
        // ������ �������� ���� �Ұ���
        controller.ActiveController(false);
    }

    private void ReturnTitle(VideoPlayer vp)
    {
        SceneManager.LoadScene(0);
    }
}
