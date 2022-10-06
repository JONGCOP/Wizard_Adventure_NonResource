using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGMPlayer : Singleton<BGMPlayer>
{
    public enum BGM_Type
    {
        NonBattle,
        TutoBattle,
        MainBattle,
        Boss,
        Count
    }

    [SerializeField] private float fadeTime = 3.0f;
    [SerializeField, Range(0, 1)] 
    private float volumeSize = 1.0f;
    private AudioClip mainBGM;
    private float currTime;

    public float VolumeSize
    {
        get => volumeSize;
        set => volumeSize = value;
    }

    [SerializeField, EnumNamedArray(typeof(BGM_Type))]
    private AudioClip[] bgmClips = new AudioClip[(int)BGM_Type.Count];

    private AudioSource audioSource;

    protected override void OnAwake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // ���� �����ϰ� ���� ������ BGM �÷���
    public void PlayBGM(BGM_Type bgm)
    {
        mainBGM = bgmClips[(int)bgm];
        audioSource.clip = bgmClips[(int)bgm];
        audioSource.Play();
    }

    // ������ �÷����ϴ� ������ �ٲ㼭 ���
    // �ٲٴ� ���� �� ������ Fade In / Fade Out
    public void Change(BGM_Type bgm)
    {
        Sequence s = DOTween.Sequence();
        s.Append(audioSource.DOFade(0, fadeTime));
        s.AppendCallback(() => {
            currTime = audioSource.time;
            audioSource.Stop();
            audioSource.time = 0;
            audioSource.clip = bgmClips[(int)bgm];
            audioSource.Play();
        });
        s.Append(audioSource.DOFade(volumeSize, fadeTime));
    }

    // ������ �÷����ϴ� �������� �ٲٱ�
    public void Rollback()
    {
        Sequence s = DOTween.Sequence();
        s.Append(audioSource.DOFade(0, fadeTime));
        s.AppendCallback(() => {
            audioSource.clip = mainBGM;
            audioSource.Play();
            audioSource.time = currTime;
        });
        s.Append(audioSource.DOFade(volumeSize, fadeTime));
    }

    // BGM ���߱�
    public void Stop()
    {
        Sequence s = DOTween.Sequence();
        s.Append(audioSource.DOFade(0, fadeTime));
        s.onComplete = () => {
            audioSource.Stop();
        };
    }
}
