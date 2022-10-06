using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 그립 방식의 모든 마법을 담을 수 있게 만든 클래스
/// 작성자 - 차영철
/// </summary>
public abstract class GripMagic : Magic
{
    public abstract void TurnOn();

    public abstract void TurnOff();

    public virtual void SetTarget() {}
}
