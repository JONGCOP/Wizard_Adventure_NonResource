using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Ʃ�丮��(Ȥ�� ����) ���� ������ ������ �����ϴ� Ŭ����
/// �ۼ��� - ����ö
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
