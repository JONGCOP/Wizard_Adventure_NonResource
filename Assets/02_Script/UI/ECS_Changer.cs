using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ӽ� ��ȯ �� ���� ����
/// �ۼ��� - �赵��
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
        print("���� ���� : " + num);
        audioSource.PlayOneShot(elementSound[num]); 
    }

    public void SoundChangeRotate(int num)
    {
        audioSource.PlayOneShot(rotateSound[num]);
    }

}
