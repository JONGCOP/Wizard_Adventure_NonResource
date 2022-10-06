using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// 전투 시 상태 안내 UI
/// </summary>
public class BattleUI : Singleton<BattleUI>
{
    [Header("Room")]
    [SerializeField, Tooltip("프리팹")]
    private GameObject enemyCountObj;
    [SerializeField, Tooltip("Text")]
    private TextMeshProUGUI enemyCountText;

    [Header("Boss")]
    [SerializeField, Tooltip("타겟")]
    private CharacterStatus bossStatus;
    [SerializeField, Tooltip("보스 체력바 오브젝트")]
    private GameObject hpBar;
    private CanvasGroup hpBarCanvasGroup;
    [SerializeField]
    private Image hpFillImage;
    [SerializeField, Tooltip("체력바 애니메이션 이미지")]
    private Image hpAnimImage;

    [Header("Warning")] 
    [SerializeField, Tooltip("주의 메세지 및 알림")]
    private TextMeshProUGUI warningText;

    private void Awake()
    {
        hpBarCanvasGroup = hpBar.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        bossStatus.onHpChange += OnHpChanged;
        bossStatus.onDead += OnBossDead;
    }

    private void Update()
    {
        hpAnimImage.fillAmount = Mathf.Lerp(hpAnimImage.fillAmount, hpFillImage.fillAmount, 5 * Time.deltaTime);
    }

    private void OnHpChanged(float percent)
    {
        hpFillImage.fillAmount = percent;
    }

    private void OnBossDead()
    {
        bossStatus.onHpChange -= OnHpChanged;
        hpBar.SetActive(false);
        hpBarCanvasGroup.DOFade(0, 0.5f);
    }

    public void OnBossBattleStart()
    {
        hpBar.SetActive(true);
        hpBarCanvasGroup.alpha = 0;
        hpBarCanvasGroup.DOFade(1, 0.5f);
    }

    public void OnBossBattleEnd()
    {
        Sequence s = DOTween.Sequence();
        s.Append(hpBarCanvasGroup.DOFade(0, 0.5f));
        s.onComplete = () =>
        {
            hpBar.SetActive(false);
        };
    }

    public void WarningMessage(float time, string context)
    {

    }

    public void SetRemainCount(int remain, int max)
    {
        if (remain <= 0)
        {
            enemyCountObj.gameObject.SetActive(false);
        }
        else
        {
            enemyCountObj.gameObject.SetActive(true);
            enemyCountText.text = $"{remain}/{max}";
        }
    }
}
