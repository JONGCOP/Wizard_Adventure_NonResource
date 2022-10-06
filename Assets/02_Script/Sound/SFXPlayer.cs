using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SFX 출력을 위한 범용 AudioSource Object Pool 클래스
/// 짧은 시간동안 출력하는 SFX들을 기준으로 한다
/// </summary>
public class SFXPlayer : Singleton<SFXPlayer>
{
    public AudioSource sfxPrefab;
    public int poolSize = 15;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    // Pool 초기화
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var sfx = Instantiate(sfxPrefab);
            sfxPrefab.gameObject.SetActive(false);
            sfxPool.Enqueue(sfx);
        }
    }

    /// <summary>
    /// 효과음을 위한 AudioSource 가져오기
    /// </summary>
    public AudioSource GetSFX()
    {
        // SFX는 짧은 시간 출력되고 다시 실행되지 않을 것이라 예상됨
        // 그러므로 최근에 사용하지 않은 sfx를 넘겨주고
        // 해당 sfx를 가장 최근에 사용한 오브젝트로 처리하여
        // 순환식으로 sfx를 사용하는 구조로 만듦
        AudioSource sfx;
        if (sfxPool.Count > 0)
        {
            sfx = sfxPool.Dequeue();
        }
        else
        {
            sfx = Instantiate(sfxPrefab);
        }

        sfx.gameObject.SetActive(true);
        sfxPool.Enqueue(sfx);

        return sfx;
    }

    /// <summary>
    /// 특정 위치를 기준으로 소리를 재생한다
    /// (이 경우 위치에 따라 소리의 크기가 다르게 들린다)
    /// </summary>
    /// <param name="position">재생 위치</param>
    /// <param name="sound">재생할 소리</param>
    public void PlaySpatialSound(Vector3 position, AudioClip sound)
    {
        var sfx = GetSFX();
        sfx.transform.position = position;
        sfx.spatialBlend = 1.0f;
        sfx.PlayOneShot(sound);
    }

    /// <summary>
    /// 위치에 상관없이 일정한 볼륨으로 소리를 재생한다
    /// </summary>
    /// <param name="sound"></param>
    public void PlayNonSpatialSound(AudioClip sound)
    {
        var sfx = GetSFX();
        sfx.spatialBlend = 0.0f;
        sfx.PlayOneShot(sound);
    }
}
