using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// 도움말 창 기능 구현
/// 작성자 - 차영철
/// </summary>
public class HelpWindow : MonoBehaviour
{
    [SerializeField] protected Button prevButton;
    [SerializeField] protected Button nextButton;

    [SerializeField] protected TutorialViewer currentViewer;
    [SerializeField] protected TutorialViewer nextViewer;

    [SerializeField] protected TutorialExplainData[] contents;
    protected int currentIndex = 0;

    [SerializeField] protected TextMeshProUGUI indexViewer;

    [Header("Animation Setting")] 
    [SerializeField] protected float animTime = 0.5f;
    [SerializeField] protected float movePosX = 1024f;

    [SerializeField] protected AudioSource audioSource;    
    [SerializeField] protected AudioClip[] pageSound;
    

    protected bool isPlayingAnimation = false;
    
    // 초기화
    public void Open()
    {
        // 현재 페이지 세팅 후 실행
        currentViewer.SetContext(contents[currentIndex]);
        // Fade In Animation?
        currentViewer.Play();

        audioSource = gameObject.GetComponent<AudioSource>();
        SoundPlay(2);
        // 이전 페이지나 다음 페이지로 이동할 수 있는지 확인
        prevButton.interactable = currentIndex != 0;
        nextButton.interactable = currentIndex != contents.Length - 1;
    }

    private void OnDisable()
    {
        currentViewer.Stop();
    }

    private void Update()
    {
        // 방향키를 이용한 조작
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadPrevTutorial();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNextTutorial();
        }
        
    }

    public void LoadPrevTutorial()
    {
        // 키보드(XR Controller)로 누르면 버튼을 막았어도 함수가 동작할 가능성이 있음
        // 고로 Index == 0이면 함수 동작을 막아야 한다
        if (currentIndex == 0 || isPlayingAnimation)
        {
            return;
        }

        // 현재 뷰어와 다음 뷰어 작동 정지
        nextViewer.gameObject.SetActive(true);     
        nextViewer.SetContext(contents[currentIndex - 1]);
        currentViewer.Stop();
        nextViewer.Stop();

        // 다음 뷰어 위치 변경 후 슬라이드 애니메이션 
        ChangeViewer(-movePosX);
        SetCurrentIndex(currentIndex - 1);
        SoundPlay(0);
    }

    public void LoadNextTutorial()
    {
        // 키보드(XR Controller)로 누르면 버튼을 막았어도 함수가 동작할 가능성이 있음
        // 고로 Index == 0이면 함수 동작을 막아야 한다
        if (currentIndex == contents.Length - 1 || isPlayingAnimation)
        {
            return;
        }

        // 현재 뷰어와 다음 뷰어 작동 정지 및 초기화
        nextViewer.gameObject.SetActive(true);
        nextViewer.SetContext(contents[currentIndex + 1]);
        currentViewer.Stop();
        nextViewer.Stop();

        ChangeViewer(movePosX);
        SetCurrentIndex(currentIndex + 1);
        SoundPlay(1);
    }

    protected void ChangeViewer(float translatePosX)
    {
        isPlayingAnimation = true;

        DOTween.defaultTimeScaleIndependent = true;
        DOTween.timeScale = 1.0f;

        // 다음 뷰어 위치 변경 후 슬라이드 애니메이션
        Sequence s = DOTween.Sequence();
        s.timeScale = 1.0f;
        nextViewer.transform.localPosition = Vector3.right * translatePosX;
        s.Append(nextViewer.transform.DOLocalMoveX(0, animTime));
        s.Join(currentViewer.transform.DOLocalMove(Vector3.left * translatePosX, animTime));
        s.Join(nextViewer.GetComponent<CanvasGroup>().DOFade(1,1));
        s.Join(currentViewer.GetComponent<CanvasGroup>().DOFade(0, 1));
        s.OnComplete(() => {
            SwapCurrentAndNext();
            currentViewer.Play();
            nextViewer.Stop();
            isPlayingAnimation = false;
        });
        //s.Play();
        s.SetUpdate(true);
    }

    private void SwapCurrentAndNext()
    {
        (currentViewer, nextViewer) = (nextViewer, currentViewer);
    }

    protected void SetCurrentIndex(int value)
    {
        currentIndex = value;

        prevButton.interactable = currentIndex != 0;
        nextButton.interactable = currentIndex != contents.Length - 1;

        indexViewer.text = $"{value + 1} / {contents.Length}";
    }

    public void SoundPlay(int num)
    {
        audioSource.PlayOneShot(pageSound[num]);        
    }

}
