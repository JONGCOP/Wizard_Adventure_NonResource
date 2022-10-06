using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 디버그용 - 손(VR 컨트롤러) 위치
/// </summary>
public class MouseToHandAdapter : MonoBehaviour
{
    private Camera eyeCamera;

    public LineRenderer line;
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private Transform footPos;

    public Transform rightHandPos;

    public BoxCollider hitCollider;

    private RaycastHit hit;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        eyeCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            hitCollider.gameObject.SetActive(true);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            hitCollider.gameObject.SetActive(false);
        }

        Ray ray = eyeCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            line.SetPosition(0, eyeCamera.transform.position);
            line.SetPosition(1, hit.point);

            rightHandPos.position = hit.point;
        }

        if (playerInput.actions["Teleport"].WasPressedThisFrame())
        {
            line.gameObject.SetActive(true);
            teleportTarget.gameObject.SetActive(true);
            print("Get Down Teleport");
        }
        if (playerInput.actions["Teleport"].WasReleasedThisFrame())
        {
            print("Get Up Teleport");
            line.gameObject.SetActive(false);
            teleportTarget.gameObject.SetActive(false);

            transform.position = line.GetPosition(1) - footPos.localPosition;
        }
    }
}
