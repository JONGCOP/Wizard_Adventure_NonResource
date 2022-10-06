using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.InputSystem;

/// <summary>
/// 전체 Window를 관리하는 클래스로, 모든 Window는 이 클래스를 거쳐서 켜야한다
/// Window - 게임을 중단하고 열리는 UI
/// 작성자 - 차영철
/// </summary>
[DisallowMultipleComponent]
public class WindowSystem : Singleton<WindowSystem>
{
    class WindowClass
    {
        public GameObject windowObject;
        public bool isUserExitable;

        public WindowClass(GameObject windowObject, bool isUserExitable)
        {
            this.windowObject = windowObject;
            this.isUserExitable = isUserExitable;
        }
    }

    // 최상단 Window를 구분
    private Stack<WindowClass> windowStack = new Stack<WindowClass>();

    [SerializeField]
    private TutorialWindow _tutorialWindow;

    public static TutorialWindow tutorialWindow { get; private set; }

   
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject gameOver;
    public GameObject Loading;
    private CanvasGroup cg;

    private PlayerInput playerInput;
    private PlayerController playerController;

    protected override void OnAwake()
    {
        base.OnAwake();
        tutorialWindow = _tutorialWindow;
    }

    void Start()
    {
        Debug.Log("Window System Activate");
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.timeScale = 1.0f;
        playerController = GameManager.Controller;
    }

    private void Update()
    {
        //if (playerInput.actions[""].WasPressedThisFrame())
        //{
        //    // 창 닫을거 없으면 메뉴 켜기
        //    if (windowStack.Count == 0)
        //    {
        //        OpenWindow(menu, true);
        //    }
        //    // 창 닫기
        //    else
        //    {
        //        CloseWindow(true);
        //    }
        //}
    }

    public void Menu()
    {
        // 창 닫을거 없으면 메뉴 켜기
        if (windowStack.Count == 0)
        {
            OpenWindow(menu, true);
        }
        // 창 닫기
        else
        {
            CloseWindow(true);
        }
    }

    public void OpenWindow(GameObject windowObject, bool isUserExitable)
    {
        if (windowStack.Count == 0)
        {
            SetWindowMode(true);
        }
        windowObject.SetActive(true);
        windowStack.Push(new WindowClass(windowObject, isUserExitable));
    }

    public bool CloseWindow(bool isUserExit)
    {
        // 더 이상 윈도우가 켜져 있지 않을 땐 윈도우를 끌 수 없다
        if (windowStack.Count == 0)
        {
            return false;
        }

        var wc = windowStack.Pop();
        // 종료할 수 있는 Window인지 확인한다
        if (isUserExit && !wc.isUserExitable)
        {
            windowStack.Push(wc);
            return false;
        }

        // 윈도우 종료
        wc.windowObject.SetActive(false);

        // 윈도우를 전부 종료했다면 커서를 띄운다
        if (windowStack.Count == 0)
        {
            SetWindowMode(false);
        }

        return true;
    }

    public void SetWindowMode(bool display)
    {
        // UI 보여줄 때는 게임이 멈춰야함
        Time.timeScale = display ? 0 : 1;

        // 컨트롤러 세팅도 UI에 맞게 변경
        playerController.SetMode(!display);
    }
}
