using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// UI에 현재 프레임 레이트를 보여준다
/// 작성자 : 차영철
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
