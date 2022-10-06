using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 투사체 공격을 감지해서 막는 기능
/// 작성자 - 차영철
/// </summary>
public class MagicShield : MonoBehaviour
{
    [SerializeField, Tooltip("방패 생성/해제 시 설정할 CutOut - 초기 / 활성화")]
    private Vector2 cutoutValue = new Vector2(1.0f, 0.5f);

    [SerializeField, Tooltip("투사체 막았을 때 진동 시간 및 세기")]
    private Vector3 blockVibrationValue = new Vector3(0.1f, 0.2f, 0.2f);

    [SerializeField, Tooltip("투사체를 막았을 때 이펙트")]
    private GameObject blockEffectPrefab;

    [SerializeField]
    private Material shieldObjMat;
    private readonly int cutoutID = Shader.PropertyToID("_CutOut");

    [Header("SFX")]
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        shieldObjMat.SetFloat(cutoutID, cutoutValue.x);
        shieldObjMat.DOFloat(cutoutValue.y, cutoutID, 0.3f);

        SFXPlayer.Instance.PlaySpatialSound(transform.position, turnOnSound);
    }

    private void Start()
    {
        // 최대 3개 정도 막을거라 예상
        PoolSystem.Instance.InitPool(blockEffectPrefab, 3);
    }

    /// <summary>
    /// 방패 비활성화 시 사용
    /// </summary>
    public void TurnOff()
    {
        Sequence s = DOTween.Sequence();
        s.Append(shieldObjMat.DOFloat(cutoutValue.x, cutoutID, 0.3f));
        s.onComplete = () => gameObject.SetActive(false);

        SFXPlayer.Instance.PlaySpatialSound(transform.position, turnOffSound);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print($"Collision Enter - {collision.gameObject.name}");
        // 막았을 때 진동 주기
        VibrationManager.Instance.SetVibration(blockVibrationValue.x,
            blockVibrationValue.y, blockVibrationValue.z, VibrationManager.ControllerType.LeftTouch);
        CreateBlockEffect(collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void CreateBlockEffect(Vector3 position, Vector3 normal)
    {
        var blockEffect = PoolSystem.Instance.GetInstance<GameObject>(blockEffectPrefab);
        blockEffect.transform.position = position;
        blockEffect.transform.forward = normal;
    }
}
