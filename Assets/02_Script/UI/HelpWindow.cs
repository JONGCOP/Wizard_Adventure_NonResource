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
/// ���� â ��� ����
/// �ۼ��� - ����ö
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
    
    // �ʱ�ȭ
    public void Open()
    {
        // ���� ������ ���� �� ����
        currentViewer.SetContext(contents[currentIndex]);
        // Fade In Animation?
        currentViewer.Play();

        audioSource = gameObject.GetComponent<AudioSource>();
        SoundPlay(2);
        // ���� �������� ���� �������� �̵��� �� �ִ��� Ȯ��
        prevButton.interactable = currentIndex != 0;
        nextButton.interactable = currentIndex != contents.Length - 1;
    }

    private void OnDisable()
    {
        currentViewer.Stop();
    }

    private void Update()
    {
        // ����Ű�� �̿��� ����
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
        // Ű����(XR Controller)�� ������ ��ư�� ���Ҿ �Լ��� ������ ���ɼ��� ����
        // ��� Index == 0�̸� �Լ� ������ ���ƾ� �Ѵ�
        if (currentIndex == 0 || isPlayingAnimation)
        {
            return;
        }

        // ���� ���� ���� ��� �۵� ����
        nextViewer.gameObject.SetActive(true);     
        nextViewer.SetContext(contents[currentIndex - 1]);
        currentViewer.Stop();
        nextViewer.Stop();

        // ���� ��� ��ġ ���� �� �����̵� �ִϸ��̼� 
        ChangeViewer(-movePosX);
        SetCurrentIndex(currentIndex - 1);
        SoundPlay(0);
    }

    public void LoadNextTutorial()
    {
        // Ű����(XR Controller)�� ������ ��ư�� ���Ҿ �Լ��� ������ ���ɼ��� ����
        // ��� Index == 0�̸� �Լ� ������ ���ƾ� �Ѵ�
        if (currentIndex == contents.Length - 1 || isPlayingAnimation)
        {
            return;
        }

        // ���� ���� ���� ��� �۵� ���� �� �ʱ�ȭ
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

        // ���� ��� ��ġ ���� �� �����̵� �ִϸ��̼�
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
