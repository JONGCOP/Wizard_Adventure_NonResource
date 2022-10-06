using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy 체력바를 어느 각도에서 보더라도 똑바로 볼 수 있도록 하기
/// 작성자 - 성종현
/// </summary>
public class Billboard : MonoBehaviour
{
    Transform camTransform;
    [Tooltip("HPbar가 쳐다보는 대상")]
    public Transform lookAt;
    // Start is called before the first frame update

    private void Awake()
    {
        
    }

    void Start()
    {
        //camTransform = Camera.main.transform;
    }

    void Update()
    {
        //transform.rotation = camTransform.rotation;
        this.transform.LookAt(lookAt);
    }
}
