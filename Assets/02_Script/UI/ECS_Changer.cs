using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 속성 변환 시 사운드 변경
/// 작성자 - 김도영
/// </summary>

public class ECS_Changer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] elementSound;
    [SerializeField] private AudioClip[] rotateSound;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();      
    }

    public void SoundChange(int num)
    {
        print("현재 선곡 : " + num);
        audioSource.PlayOneShot(elementSound[num]); 
    }

    public void SoundChangeRotate(int num)
    {
        audioSource.PlayOneShot(rotateSound[num]);
    }

}
