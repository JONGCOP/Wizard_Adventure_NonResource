using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// ��� ������ �⺻�� �Ǵ� Ŭ����
/// �ۼ��� - ����ö
/// </summary>
public abstract class Magic : MonoBehaviour
{
    [SerializeField, Tooltip("���� �������� ��ġ�� ������")] 
    private bool isSelfTarget = false;
    
    [SerializeField, Tooltip("Object Pool ũ��")]
    private int poolSize = 1;


    public bool IsSelfTarget => isSelfTarget;
    public int PoolSize => poolSize;

    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public virtual void SetDirection(Vector3 dir)
    {
        transform.forward = dir;
    }

    /// <summary>
    /// ������ �� �����ϰ� �����ϴ� �Լ�
    /// 
    /// </summary>
    public virtual void StartMagic()
    {
    }
}
