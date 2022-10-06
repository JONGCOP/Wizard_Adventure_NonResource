using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTutorialOpener : MonoBehaviour
{
    public TutorialWindow tutorialWindow;

    private WindowSystem windowSystem;
    private void Start()
    {
        windowSystem = WindowSystem.Instance;
    }

    void Update()
    {
        // 튜토리얼 윈도우 켜기
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tutorialWindow.gameObject.activeSelf)
            {
                windowSystem.CloseWindow(true);
            }
            else
            {
                windowSystem.OpenWindow(tutorialWindow.gameObject, true);
            }
        }
    }
}
