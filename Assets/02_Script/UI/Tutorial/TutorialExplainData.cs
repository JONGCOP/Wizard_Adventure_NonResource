using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 튜토리얼(혹은 도움말) 설명 페이지 정보를 저장하는 클래스
/// 작성자 - 차영철
/// </summary>
[CreateAssetMenu(fileName = "New Tutorial Explain Data", menuName = "Game Data/Tutorial Data")]
public class TutorialExplainData : ScriptableObject
{
    public VideoClip clip;
    public string explain_Title;
    [TextArea(3, 5)]
    public string explain;
    public Sprite controllerImage_L;
    public Sprite controllerImage_R;

}
