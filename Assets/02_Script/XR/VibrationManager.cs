using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʈ�ѷ� ������ �����ϴ� Ŭ����
/// </summary>
public class VibrationManager : Singleton<VibrationManager>
{
    public enum ControllerType
    {
        LeftTouch = 0x01,
        RightTouch = 0x02,
        All = LeftTouch | RightTouch
    }

    // �¿� ��Ʈ�ѷ� ���� �ڷ�ƾ
    private Coroutine[] controllerCoroutine = new Coroutine[(int)ControllerType.RightTouch];

    public void SetVibration(float waitTime, float frequency, float amplitude, ControllerType controllerType)
    {
        int leftTouch = (int)controllerType & (int)ControllerType.LeftTouch;
        if (leftTouch > 0)
        {
            int leftTouchIndex = leftTouch - 1;
            if (controllerCoroutine[leftTouchIndex] != null)
            {
                StopCoroutine(controllerCoroutine[leftTouchIndex]);
            }
            controllerCoroutine[leftTouchIndex] = 
                StartCoroutine(IEVibration(waitTime, frequency, amplitude, leftTouch));
        }
        int rightTouch = (int)controllerType & (int)ControllerType.RightTouch;
        if (rightTouch > 0)
        {
            int rightTouchIndex = rightTouch - 1;
            if (controllerCoroutine[rightTouchIndex] != null)
            {
                StopCoroutine(controllerCoroutine[rightTouchIndex]);
            }
            controllerCoroutine[rightTouchIndex] =
                StartCoroutine(IEVibration(waitTime, frequency, amplitude, rightTouch));
        }
    }

    private IEnumerator IEVibration(float waitTime, float frequency, float amplitude, int controller)
    {
        print($"controller {(OVRInput.Controller)controller} vibrationed");
        OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)controller);
        yield return new WaitForSeconds(waitTime);
        OVRInput.SetControllerVibration(0, 0, (OVRInput.Controller)controller);

        controllerCoroutine[controller - 1] = null;
    }
}
