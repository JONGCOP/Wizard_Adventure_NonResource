using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// 튜토리얼 설명 페이지 동작 클래스
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
        // Tutorial Data로부터 정보 가져와서 적용하기
        videoPlayer.clip = explainData.clip;
        explain_titleText.text = explainData.explain_Title;
        explainText.text = explainData.explain;
        controllerImageL.sprite = explainData.controllerImage_L;
        controllerImageR.sprite = explainData.controllerImage_R;
    }

    public void Play()
    {
        // 영상 처음부터 재생
        videoPlayer.frame = 0;
        videoPlayer.Play();
    }

    public void Stop()
    {
        // 영상 정지
        videoPlayer.Stop();
    }
}
