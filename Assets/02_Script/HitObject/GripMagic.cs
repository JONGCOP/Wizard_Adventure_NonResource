using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �׸� ����� ��� ������ ���� �� �ְ� ���� Ŭ����
/// �ۼ��� - ����ö
/// </summary>
public abstract class GripMagic : Magic
{
    public abstract void TurnOn();

    public abstract void TurnOff();

    public virtual void SetTarget() {}
}
