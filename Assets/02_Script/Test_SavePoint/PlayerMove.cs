using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public float speed = 5f;
    // Update is called once per frame
    void Update()
    {
        // ������� �Է¿� ���� �����¿�� �̵��ϰ� �ʹ�.

        // 1. ������� �Է¿� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. �����¿��� ������ �����
        Vector3 dir = new Vector3(h, 0, v);

        // dir�� ũ�⸦ 1�� ���� �ʿ䰡 ����. ����ȭ����
        dir.Normalize();

        // 3. �� �������� �̵��ϰ� �ʹ�.

        // �̵����� (P = P0 + vt) --> (P += vt)
        // �ӵ� Velocity  ==> ���� + ũ��
        // �ӷ� Speed ==> ũ��

        transform.position += dir * speed * Time.deltaTime;
        //vector -- �������� ����
    }
}
