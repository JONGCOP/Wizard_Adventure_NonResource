using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SFX ����� ���� ���� AudioSource Object Pool Ŭ����
/// ª�� �ð����� ����ϴ� SFX���� �������� �Ѵ�
/// </summary>
public class SFXPlayer : Singleton<SFXPlayer>
{
    public AudioSource sfxPrefab;
    public int poolSize = 15;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    // Pool �ʱ�ȭ
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
    /// ȿ������ ���� AudioSource ��������
    /// </summary>
    public AudioSource GetSFX()
    {
        // SFX�� ª�� �ð� ��µǰ� �ٽ� ������� ���� ���̶� �����
        // �׷��Ƿ� �ֱٿ� ������� ���� sfx�� �Ѱ��ְ�
        // �ش� sfx�� ���� �ֱٿ� ����� ������Ʈ�� ó���Ͽ�
        // ��ȯ������ sfx�� ����ϴ� ������ ����
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
    /// Ư�� ��ġ�� �������� �Ҹ��� ����Ѵ�
    /// (�� ��� ��ġ�� ���� �Ҹ��� ũ�Ⱑ �ٸ��� �鸰��)
    /// </summary>
    /// <param name="position">��� ��ġ</param>
    /// <param name="sound">����� �Ҹ�</param>
    public void PlaySpatialSound(Vector3 position, AudioClip sound)
    {
        var sfx = GetSFX();
        sfx.transform.position = position;
        sfx.spatialBlend = 1.0f;
        sfx.PlayOneShot(sound);
    }

    /// <summary>
    /// ��ġ�� ������� ������ �������� �Ҹ��� ����Ѵ�
    /// </summary>
    /// <param name="sound"></param>
    public void PlayNonSpatialSound(AudioClip sound)
    {
        var sfx = GetSFX();
        sfx.spatialBlend = 0.0f;
        sfx.PlayOneShot(sound);
    }
}
