using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// 모든 마법의 기본이 되는 클래스
/// 작성자 - 차영철
/// </summary>
public abstract class Magic : MonoBehaviour
{
    [SerializeField, Tooltip("누굴 기준으로 위치를 잡을지")] 
    private bool isSelfTarget = false;
    
    [SerializeField, Tooltip("Object Pool 크기")]
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
    /// 마법을 다 설정하고 시작하는 함수
    /// 
    /// </summary>
    public virtual void StartMagic()
    {
    }
}
