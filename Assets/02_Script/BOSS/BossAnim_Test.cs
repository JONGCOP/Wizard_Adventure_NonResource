using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnim_Test : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
    }

    //BaseAttack
    //Skill_1
    //ChannelingSkill
    //isChanneling
    //isDie

    // Update is called once per frame
    void Update()
    {
        AnimTestFX();
    }

    public void AnimTestFX()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetBool("BaseAttack", true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetBool("BaseAttack", false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetBool("Skill_1", true);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            anim.SetBool("Skill_1", false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            anim.SetBool("ChannelingSkill", true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            anim.SetBool("ChannelingSkill", false);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            anim.SetTrigger("isDie");
        }
    }
}
