using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PC ȯ�濡�� Ŀ�� ������ ����� ǥ���� �� �ֵ��� �����ϴ� Ŭ����
/// �ۼ��� - ����ö
/// </summary>
public class DebugPreferences : Singleton<DebugPreferences>
{
    [SerializeField] private Texture2D cursorTexture;

    private void Start()
    {

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        // ���� ���� �� ������� ���� ��Ȱ��ȭ �����̹Ƿ� Ŀ���� �� ������� �Ѵ�
        SetCursorDisplay(true);
    }

    public void SetCursorDisplay(bool display)
    {
        Time.timeScale = display ? 0 : 1;
        Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = display;
    }
}
