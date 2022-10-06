using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// UI�� ���� ������ ����Ʈ�� �����ش�
/// �ۼ��� : ����ö
/// </summary>
public class FrameCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI frameCountText;

    private static WaitForSeconds ws = new WaitForSeconds(0.1f);

    private void Start()
    {
        StartCoroutine(nameof(IEFrameCount));
    }

    IEnumerator IEFrameCount()
    {
        while (true)
        {
            int currentFrame = (int)(1.0f / Time.unscaledDeltaTime);
            frameCountText.text = currentFrame + " FPS";
            yield return ws;
        }
    }
}
