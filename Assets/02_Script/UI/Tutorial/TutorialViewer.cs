using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Ʃ�丮�� ���� ������ ���� Ŭ����
/// </summary>
public class TutorialViewer : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI explain_titleText;
    [SerializeField] private TextMeshProUGUI explainText;
    [SerializeField] private Image controllerImageL;
    [SerializeField] private Image controllerImageR;

    public void SetContext(TutorialExplainData explainData)
    {
        // Tutorial Data�κ��� ���� �����ͼ� �����ϱ�
        videoPlayer.clip = explainData.clip;
        explain_titleText.text = explainData.explain_Title;
        explainText.text = explainData.explain;
        controllerImageL.sprite = explainData.controllerImage_L;
        controllerImageR.sprite = explainData.controllerImage_R;
    }

    public void Play()
    {
        // ���� ó������ ���
        videoPlayer.frame = 0;
        videoPlayer.Play();
    }

    public void Stop()
    {
        // ���� ����
        videoPlayer.Stop();
    }
}
