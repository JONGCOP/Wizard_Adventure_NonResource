using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지 및 속성 정보
/// </summary>
[Serializable]
public class ElementDamage
{
    public int damage = 10;
    public ElementType elementType = ElementType.None;
    [Range(1, 10)]
    public int stack = 1;
}
