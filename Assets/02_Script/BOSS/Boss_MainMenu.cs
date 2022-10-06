using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss_MainMenu : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] Transform point;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();        
        MoveAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Prepend(boss.transform.DOMove(point.position, 6.0f)).onPlay = () =>
        {
            animator.SetBool("IsMove", true);
        };
        seq.onComplete = () =>
        {
            animator.SetBool("IsMove", false);
        };
    }

}
