using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform teleportTarget;

    [SerializeField] private Transform footPos;

    [SerializeField] private Camera eyeCamera;

    private RaycastHit hit;

    void Update()
    {
        if (Physics.Raycast(eyeCamera.transform.position, eyeCamera.transform.forward, out hit, 15))
        {
            line.SetPosition(0, eyeCamera.transform.position);
            line.SetPosition(1, hit.point);

            teleportTarget.position = hit.point + Vector3.up * 0.05f;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger , OVRInput.Controller.LTouch))
        {
            line.gameObject.SetActive(true);
            teleportTarget.gameObject.SetActive(true);
            print("Get Down Teleport");
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            print("Get Up Teleport");
            line.gameObject.SetActive(false);
            teleportTarget.gameObject.SetActive(false);

            transform.position = line.GetPosition(1) - footPos.localPosition;
        }
    }
}
