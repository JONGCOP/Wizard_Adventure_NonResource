using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� Ÿ��Ʋ ȭ�� ���� Ŭ����
/// ������ ����Ǹ� ���� ���� �ߴ� ȭ��
/// �ۼ��� - �赵��
/// </summary>

public class MainTitle : MonoBehaviour
{              
    [SerializeField] private GameObject back;
    [SerializeField] private AudioSource sound;
    public AudioClip[] audioClips;
    private CanvasGroup cg;    


    // Start is called before the first frame update
    void Start()
    {
        cg = back.GetComponent<CanvasGroup>();
        sound = sound.GetComponent<AudioSource>();
        sound.clip = audioClips[0]; 
        cg.alpha = 1;
        BackFade(true);
    }

    //��ŸƮ ��ư Ŭ���� �ε�ȭ�� ������ �̵�
    public void OnStart()
    {
        sound.Play();
        StartCoroutine(nameof(IESceneChange));
    }
    //����ϱ⸦ �ϸ� ���̺� ����Ʈ�� �̵�
    public void OnContinue()
    {
        sound.Play();
        print("���̺� ����Ʈ�� �̵�");
    }
    //�����ϱ⸦ ������ ������ �����
    public void OnQuit()
    {
        sound.Play();
        Application.Quit();
    }
  

    //3�� �Ŀ� �� �̵�
    IEnumerator IESceneChange()
    {
        BackFade(false);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(1);
    }
  

    //�� �̵��� ���̵� ȿ��
    private void BackFade(bool Load)
    {
        int num;
        if (cg != null)
        {
            num = Load ? 0 : 1;
            cg.DOFade(num, 3.0f);

        }
        else
        {
            cg.DOKill(); //�� �̵� �� Dotween ���� ����
        }

    }





}
