using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PC 환경에서 커서 등으로 디버그 표현할 수 있도록 관리하는 클래스
/// 작성자 - 차영철
/// </summary>
public class DebugPreferences : Singleton<DebugPreferences>
{
    [SerializeField] private Texture2D cursorTexture;

    private void Start()
    {

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        // 게임 시작 시 윈도우는 전부 비활성화 상태이므로 커서를 안 보여줘야 한다
        SetCursorDisplay(true);
    }

    public void SetCursorDisplay(bool display)
    {
        Time.timeScale = display ? 0 : 1;
        Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = display;
    }
}
