using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 투사체를 쏘는 Enemy(Archer, Sorcerer)의 발사정보
/// 작성자 - 성종현
/// </summary>
public class EnemyShooting : MonoBehaviour
{
    private Animator animator;
    public Transform attackTarget;
    public GameObject projFactory;
    public GameObject firePos;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (!attackTarget)
        {
            attackTarget = GameManager.eye;
        }
    }

    void ProjectileShooting()
    {
        GameObject projInst = Instantiate(projFactory);
        projInst.transform.position = firePos.transform.position;
        projInst.transform.LookAt(attackTarget);
        var proj = projInst.GetComponent<Projectile>();
        if (proj)
        {
            proj.StartMagic();
        }
        ParticleSystem ps = projInst.GetComponent<ParticleSystem>();
        if (ps)
        {
            ps.Stop();
            ps.Play();
        }
    }
}
