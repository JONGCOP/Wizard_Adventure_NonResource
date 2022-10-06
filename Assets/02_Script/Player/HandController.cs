using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 입력에 따라 손 동작 조절 클래스
/// 작성자 - 차영철
/// </summary>
public class HandController : MonoBehaviour
{
    /// <summary>
    /// 왼손 조작
    /// </summary>
    public enum LeftAction
    {
        Default, // 디폴트, 막기
        Teleport, // 텔레포트
    }

    public enum RightAction
    {
        Default,    // 주먹
        UiSelect,   // UI 선택 시 가리키기 액션
        WandGrip,   // 평소 완드 집었을 때
        SwordGrip,  // 얼음검
    }
    
    [SerializeField, Tooltip("왼손 애니메이션 컨트롤러")]
    private Animator leftHandAnimator;
    [SerializeField, Tooltip("오른손 애니메이션 컨트롤러")]
    private Animator rightHandAnimator;
  

    // 애니메이션 스트링 해쉬 코드
    private readonly int animNumHash = Animator.StringToHash("AnimNum");

    public void SetLeftHandAction(LeftAction action)
    {
        leftHandAnimator.SetInteger(animNumHash, (int)action);
    }

    public void SetRightHandAction(RightAction action)
    {
        rightHandAnimator.SetInteger(animNumHash, (int)action);
    }

}
