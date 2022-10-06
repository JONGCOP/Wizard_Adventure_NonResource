using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �� �Ӽ� ����
/// </summary>
[Serializable]
public class ElementDamage
{
    public int damage = 10;
    public ElementType elementType = ElementType.None;
    [Range(1, 10)]
    public int stack = 1;
}
