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
        // 사용자의 입력에 따라서 상하좌우로 이동하고 싶다.

        // 1. 사용자의 입력에 따라서
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 2. 상하좌우의 방향을 만들고
        Vector3 dir = new Vector3(h, 0, v);

        // dir의 크기를 1로 만들 필요가 있음. 정규화벡터
        dir.Normalize();

        // 3. 그 방향으로 이동하고 싶다.

        // 이동공식 (P = P0 + vt) --> (P += vt)
        // 속도 Velocity  ==> 방향 + 크기
        // 속력 Speed ==> 크기

        transform.position += dir * speed * Time.deltaTime;
        //vector -- 절대적인 방향
    }
}
