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
        // Ʃ�丮�� ������ �ѱ�
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
