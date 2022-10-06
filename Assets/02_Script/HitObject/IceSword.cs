using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// 얼음검 효과 및 이펙트 구현
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class IceSword : GripMagic
{
    [SerializeField] 
    private Vector2 cutoffHeight = new Vector2(0.0f, 3.5f);
    
    [SerializeField, Tooltip("얼음검 생성 시간")] 
    private float createTime = 1.0f;
    private float createSpeed;

    [SerializeField, Tooltip("데미지 판정 기준 속도")] 
    private float judgementSpeed = 15f;
    private float speed;
    private Vector3 previousPos;
    private Vector3 currentSwordDir;

    [SerializeField] 
    private ElementDamage elementDamage;

    [SerializeField, Tooltip("검으로 데미지 줬을 때 나오는 이펙트")] 
    private GameObject hitEffectPrefab;

    [SerializeField, Tooltip("서리 이펙트")] 
    private ParticleSystem iceFogEffect;
    [SerializeField, Tooltip("눈 휘날리는 이펙트")]
    private ParticleSystem snowEffect;

    [Header("Sound")] 
    [SerializeField, Tooltip("검 사용 시 나오는 소리")]
    private AudioClip turnOnSound;
    [SerializeField, Tooltip("검 사용 종료 후 나오는 소리")]
    private AudioClip turnOffSound;
    [SerializeField, Tooltip("검으로 상대를 맞출 때 나오는 소리")]
    private AudioClip hitSound;

    private Collider hitBox;
    private Rigidbody rigidBody;
    private Material material;
    private Light swordLight;

    private AudioSource swordAudioSource;

    private readonly int cutoffHeightID = Shader.PropertyToID("_Cutoff_Height");
    
    private void Awake()
    {
        hitBox = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
        material = GetComponent<MeshRenderer>().material;
        swordLight = GetComponentInChildren<Light>();

        swordAudioSource = GetComponentInChildren<AudioSource>();

        createSpeed = (cutoffHeight.y - cutoffHeight.x) / createTime;
    }

    private void Start()
    {
        previousPos = transform.position;

        PoolSystem.Instance.InitPool(hitEffectPrefab, 4);
    }

    private void Update()
    {
        // 검 속도 계산
        Vector3 currentPos = transform.position;
        currentSwordDir = (currentPos - previousPos).normalized;
        speed = Vector3.Distance(currentPos, previousPos) / Time.deltaTime;

        previousPos = currentPos;
    }

    public override void TurnOn()
    {
        StopCoroutine(nameof(IETurnOffSword));
        StartCoroutine(nameof(IETurnOnSword));
        VibrationManager.Instance.SetVibration(0.3f, 0.3f, 0.3f, VibrationManager.ControllerType.RightTouch);
    }

    private IEnumerator IETurnOnSword()
    {
        swordAudioSource.PlayOneShot(turnOnSound);

        float currentHeight = material.GetFloat(cutoffHeightID);
        while (currentHeight < cutoffHeight.y)
        {
            currentHeight += Time.deltaTime * createSpeed;
            material.SetFloat(cutoffHeightID, currentHeight);
            yield return null;
        }

        hitBox.enabled = true;
        iceFogEffect.gameObject.SetActive(true);
        snowEffect.gameObject.SetActive(true);
        swordLight.gameObject.SetActive(true);
    }

    public override void TurnOff()
    {
        StopCoroutine(nameof(IETurnOnSword));
        StartCoroutine(nameof(IETurnOffSword));
    }

    private IEnumerator IETurnOffSword()
    {
        swordAudioSource.PlayOneShot(turnOffSound);

        hitBox.enabled = false;
        iceFogEffect.gameObject.SetActive(false);
        snowEffect.gameObject.SetActive(false);
        swordLight.gameObject.SetActive(false);

        float currentHeight = material.GetFloat(cutoffHeightID);
        while (currentHeight > cutoffHeight.x)
        {
            currentHeight -= Time.deltaTime * createSpeed;
            material.SetFloat(cutoffHeightID, currentHeight);
            yield return null;
        }
        gameObject.SetActive(false);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // 속도를 기준으로 히트 판정
        if (speed > judgementSpeed)
        {
            var hitEffect = PoolSystem.Instance.GetInstance<GameObject>(hitEffectPrefab);
            hitEffect.transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.1f;
            hitEffect.transform.right = currentSwordDir;
            SFXPlayer.Instance.PlaySpatialSound(hitEffect.transform.position, hitSound);
            VibrationManager.Instance.SetVibration(0.3f, 0.3f, 0.3f, VibrationManager.ControllerType.RightTouch);

            // 적이면 데미지 주기
            var status = collision.collider.GetComponent<CharacterStatus>();
            if (status)
            {
                status.TakeDamage(elementDamage);
            }
        }
    }
}
