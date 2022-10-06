using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy ü�¹ٸ� ��� �������� ������ �ȹٷ� �� �� �ֵ��� �ϱ�
/// �ۼ��� - ������
/// </summary>
public class Billboard : MonoBehaviour
{
    Transform camTransform;
    [Tooltip("HPbar�� �Ĵٺ��� ���")]
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
