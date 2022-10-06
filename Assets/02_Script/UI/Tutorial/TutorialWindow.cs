using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 튜토리얼 창만 열어서 보여주는 윈도우
/// </summary>
public class TutorialWindow : HelpWindow
{
    [SerializeField] private CanvasGroup canvasGroup;        
    [SerializeField] private Button exitButton;    


    public void Open(TutorialExplainData[] datas)
    {
        exitButton.interactable = false;
        contents = datas;
        SoundPlay(1);
        SetCurrentIndex(0);
        // 현재 페이지 세팅
        currentViewer.SetContext(datas[currentIndex]);
        
        // 튜토리얼 창이 켜질 땐 index 0부터 시작해야 한다
        // 이전 페이지나 다음 페이지로 이동할 수 있는지 확인
        // Fade In
        canvasGroup.alpha = 0.0f;
       

        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(1.0f, 0.5f)).SetUpdate(true);
        s.onComplete = () =>
        {
            currentViewer.Play();            
            exitButton.interactable = true;
        };
    }

    public void Close()
    {
        print("Tutorial Closed");
        currentViewer.Stop();        
        canvasGroup.alpha = 1.0f;
        SoundPlay(0);
        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(0.0f, 0.5f)).SetUpdate(true);
        s.onComplete = () => {
            
            WindowSystem.Instance.CloseWindow(true);            
        };
    }

}
