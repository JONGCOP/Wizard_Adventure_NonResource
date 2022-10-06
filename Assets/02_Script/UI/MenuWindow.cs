using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �޴� ��� ���� Ŭ����
/// �ۼ��� - ����ö
/// </summary>
public class MenuWindow : MonoBehaviour
{
    [SerializeField] private HelpWindow helpWindow;


    public void OnLoad()
    {
        print("���̺� ����Ʈ�� �̵�");
        helpWindow.SoundPlay(2);
    }

    public void OpenHelp()
    {
        helpWindow.SoundPlay(2);
        WindowSystem.Instance.OpenWindow(helpWindow.gameObject, true);
    }
    
    public void CloseMenu()
    {
        helpWindow.SoundPlay(3);
        // WindowSystem�� ȣ���� â�� �ݴ´�
        WindowSystem.Instance.CloseWindow(true);
    }

    public void OnMainMenu()
    {
        
        helpWindow.SoundPlay(2);
        SceneManager.LoadScene("MainTitle");
    }

    public void OnEnable()
    {
        helpWindow.SoundPlay(3);
    }


}
