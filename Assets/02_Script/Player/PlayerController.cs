using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class PlayerController : Singleton<PlayerController>
{
    public enum MagicAbility
    {
        Base = 1,
        Shield = 2,
        Grip = 4,
        Charge = 8,
        ChangeElement = 16,
    }

    [SerializeField]
    private MagicShield magicShield;

    [Header("For Debug")] 
    private bool isVR;    
    [SerializeField] private float handPosZ = 0.3f;
    [SerializeField] private float pressedHandPosZ = 0.5f;
    [SerializeField] private float releasedHandPosZ = 0.3f;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    

    private Camera _main;
    
    private Vector2 m_Move;
    

    private int previousChangeElementInput = 0;
    public int pp_Alpha = 0;

    private PlayerInput playerInput;
    private PlayerMoveRotate playerMoveRotate;
    private PlayerMagic playerMagic;
    private PropertiesWindow playerProprerties;    

    // 배운 능력(비트마스크 방식)
    [SerializeField, Tooltip("배운 능력")]
    private int learnedAbility = 0;

    public bool CanControlPlayer => playerInput.currentActionMap.name.Equals("Player");

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMoveRotate = GetComponent<PlayerMoveRotate>();
        playerMagic = GetComponent<PlayerMagic>();
        playerProprerties =
            transform.FindChildRecursive("Properties").gameObject.GetComponent<PropertiesWindow>();
        Debug.Assert(magicShield, "Error : magic shield not set");
    }

    private void Start()
    {
        _main = Camera.main;
        Debug.Log(playerInput.currentActionMap.name);

        isVR = IsPresent();
        var inputModule = GameObject.FindObjectOfType<UnityEngine.EventSystems.OVRInputModule>();
        var inputSystemUI = GameObject.FindObjectOfType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (isVR)
        {
            inputModule.enabled = true;
            inputSystemUI.enabled = false;
        }
        else
        {
            inputModule.enabled = false;
            inputSystemUI.enabled = true;
        }
        //OVRManager.display.RecenterPose();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_Move = context.ReadValue<Vector2>();
    }

    public void Update()
    {
        if (!isVR)
        {
            MousePosToHandPos();
            ShootMagic();
            RotateX();
        }

        // 플레이어 마법
        ChangeElement();
        Grip();
        Charge();

        // 투사체 방어
        Shield();

        RotateY();
        Move(m_Move);
        Teleport();

        //속성 선택 창 On/Off
        //PropertieseAcitve();        

        // 메뉴
        Menu();
    }

    public static bool IsPresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }

    public void SetMode(bool isBattle)
    {
        // UI 보여줄 때는 이동, 회전, 마법을 사용하면 안 됨
        var actionMap = isBattle ? "Player" : "UI";
        playerInput.SwitchCurrentActionMap(actionMap);
        playerMagic.SetHand(isBattle);
    }

    public void ActiveController(bool isActive)
    {
        if (isActive)
        {
            playerInput.actions.Enable();
        }
        else
        {
            playerInput.actions.Disable();
        }
    }

    // 마우스 위치를 손 위치로 적용
    private void MousePosToHandPos()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 mousePosition = Mouse.current.position.ReadValue();

#else
        Vector2 mousePosition = Input.mousePosition;
#endif

        // 깊이에 따라
        float h = Screen.height;
        float w = Screen.width;
        float screenSpacePosX = (mousePosition.x - (w * 0.5f)) / w * 2;
        float screenSpacePosY = (mousePosition.y - (h * 0.5f)) / h * 2;
        leftHandTransform.localPosition = new Vector3(screenSpacePosX * handPosZ, screenSpacePosY * handPosZ, handPosZ);
        rightHandTransform.localPosition = new Vector3(screenSpacePosX * handPosZ, screenSpacePosY * handPosZ, handPosZ);

        // 눈 위치를 기준으로 손의 방향을 정함
        Vector3 eyePos = _main.transform.position;
        leftHandTransform.forward = (leftHandTransform.position - eyePos).normalized;
        rightHandTransform.forward = (rightHandTransform.position - eyePos).normalized;
    }

    private void ShootMagic()
    {
        if (playerInput.actions["Shoot Magic"].WasPressedThisFrame())
        {
            handPosZ = pressedHandPosZ;
        }
        if (playerInput.actions["Shoot Magic"].WasReleasedThisFrame())
        {
            handPosZ = releasedHandPosZ;
        }
    }

    private void Grip()
    {
        if (!CheckLearnedAbility(MagicAbility.Grip))
        {
            return;
        }

        if (playerInput.actions["Grip"].WasPressedThisFrame())
        {
            playerMagic.TurnOnGrip();
        }
        if (playerInput.actions["Grip"].WasReleasedThisFrame())
        {
            playerMagic.TurnOffGrip();
        }
    }

    private void Charge()
    {
        if (!CheckLearnedAbility(MagicAbility.Charge))
        {
            return;
        }

        if (playerInput.actions["Charge"].WasPressedThisFrame())
        {
            playerMagic.StartCharge();
        }
        if (playerInput.actions["Charge"].IsPressed())
        {
            playerMagic.OnCharge();
        }
        if (playerInput.actions["Charge"].WasReleasedThisFrame())
        {
            playerMagic.EndCharge();
        }
    }

    private void ChangeElement()
    {
        if (!CheckLearnedAbility(MagicAbility.ChangeElement))
        {
            return;
        }
        
        int input = Mathf.RoundToInt(playerInput.actions["Change Element"].ReadValue<float>());
        if (input != previousChangeElementInput)
        {                 
            playerMagic.ChangeElement(input);            
            playerProprerties.OnChangeElement(input);                             //속성 선택 입력 값을 받음     
            playerProprerties.OnPropertise(1);                                              //프로퍼티 창이 뜸            
        }
        previousChangeElementInput = input;        
    }

    private void Shield()
    {
        if (!CheckLearnedAbility(MagicAbility.Shield))
        {
            return;
        }

        if (playerInput.actions["Shield"].WasPressedThisFrame())
        {
            magicShield.gameObject.SetActive(true);
        }
        if (playerInput.actions["Shield"].WasReleasedThisFrame())
        {
            magicShield.TurnOff();
        }
    }

    #region Movement
    private void Teleport()
    {
        if (playerInput.actions["Teleport"].WasPressedThisFrame())
        {
            playerMoveRotate.StartTeleport();
        }
        if (playerInput.actions["Teleport"].IsPressed())
        {
            playerMoveRotate.OnTeleport();
        }
        if (playerInput.actions["Teleport"].WasReleasedThisFrame())
        {
            playerMoveRotate.EndTeleport();
        }
    }

    private void Move(Vector2 direction)
    {
        playerMoveRotate.Move(direction);
    }

    // 디버그용 함수
    private void RotateX()
    {
        int inputAxisY = Mathf.RoundToInt(playerInput.actions["Rotate X"].ReadValue<float>());
        playerMoveRotate.RotateX(inputAxisY);
    }

    private void RotateY()
    {
        int inputAxisX = Mathf.RoundToInt(playerInput.actions["Rotate Y"].ReadValue<float>());
        playerMoveRotate.RotateY(inputAxisX);
    }
    #endregion

    public bool CheckLearnedAbility(MagicAbility ability)
    {
        return (learnedAbility & (int)ability) > 0;
    }

    public void LearnAbility(MagicAbility ability)
    {
        learnedAbility |= (int)ability;
    }

    public bool OnClick()
    { 
        bool click = playerInput.actions["Click"].ReadValue<bool>();
        return click;
    }


    private void Menu()
    {
        if (playerInput.actions["Menu"].WasPressedThisFrame())
        {
            WindowSystem.Instance.Menu();
        }
    }
}
