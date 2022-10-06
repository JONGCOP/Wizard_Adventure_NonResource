using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// ���� �� ���� �ȳ� UI
/// </summary>
public class BattleUI : Singleton<BattleUI>
{
    [Header("Room")]
    [SerializeField, Tooltip("������")]
    private GameObject enemyCountObj;
    [SerializeField, Tooltip("Text")]
    private TextMeshProUGUI enemyCountText;

    [Header("Boss")]
    [SerializeField, Tooltip("Ÿ��")]
    private CharacterStatus bossStatus;
    [SerializeField, Tooltip("���� ü�¹� ������Ʈ")]
    private GameObject hpBar;
    private CanvasGroup hpBarCanvasGroup;
    [SerializeField]
    private Image hpFillImage;
    [SerializeField, Tooltip("ü�¹� �ִϸ��̼� �̹���")]
    private Image hpAnimImage;

    [Header("Warning")] 
    [SerializeField, Tooltip("���� �޼��� �� �˸�")]
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
