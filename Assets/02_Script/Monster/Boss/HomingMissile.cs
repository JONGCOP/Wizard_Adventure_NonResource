using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��ǥ�� �̵��ϴ� ���� �̻���
/// �ۼ��� - ����ö
/// </summary>
public class HomingMissile : Projectile
{
    private Transform target;

    [Header("Homing")]
    [SerializeField, Tooltip("���� ���� ����")] 
    private float chaseAngle = 15f;
    private float chaseAngleCos;
    [SerializeField, Tooltip("�ʴ� ȸ�� �ӵ�")] 
    private float rotateSpeed = 10f;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        Homing();
    }

    private void Homing()
    {
        var dir = (target.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dir) > chaseAngleCos)
        {
            var toRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotateSpeed * Time.deltaTime);
            rb.velocity = transform.forward * moveSpeed;
        }
    }

    public override void StartMagic()
    {
        base.StartMagic();
        target = GameManager.eye;
        chaseAngleCos = Mathf.Cos(Mathf.Deg2Rad * chaseAngle);
    }
}
