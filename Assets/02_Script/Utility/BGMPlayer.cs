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

    // 게임 시작하고 원할 때부터 BGM 플레이
    public void PlayBGM(BGM_Type bgm)
    {
        mainBGM = bgmClips[(int)bgm];
        audioSource.clip = bgmClips[(int)bgm];
        audioSource.Play();
    }

    // 기존에 플레이하던 음악을 바꿔서 재생
    // 바꾸는 동안 각 음악이 Fade In / Fade Out
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

    // 이전에 플레이하던 음악으로 바꾸기
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

    // BGM 멈추기
    public void Stop()
    {
        Sequence s = DOTween.Sequence();
        s.Append(audioSource.DOFade(0, fadeTime));
        s.onComplete = () => {
            audioSource.Stop();
        };
    }
}
