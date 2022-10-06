using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// 보스 1페이즈 코드
/// </summary>
public partial class BossFSM : MonoBehaviour
{
    public enum Phase1
    {
        Skill1,
    }

    [System.Serializable]
    private class Phase1_Skill1Data
    {
        public Transform firePosition;

        [SerializeField, Tooltip("투사체 목표 위치")] 
        public Transform targetTr;
        [SerializeField, Tooltip("투사체 생기는 위치 범위")]
        public float radius = 1.5f;
        [SerializeField, Tooltip("투사체 날릴 개수")]
        private float magicMissileCount = 3;
        [SerializeField, Tooltip("투사체 발사 간격")] 
        private float interval = 0.3f;
        [SerializeField, Tooltip("투사체 마법")] 
        private DelayedMagic delayedMagicPrefab;
        [SerializeField, Tooltip("스킬 1 음성")] 
        public AudioClip skillVoice;

        public float MagicMissileCount => magicMissileCount;
        public float Interval => interval;
        public DelayedMagic DelayedMagicPrefab => delayedMagicPrefab;
    }

    [System.Serializable]
    private class Phase1_Skill2Data
    {
        [SerializeField, Tooltip("목표 추적하는 시간")]
        public float aimTime = 3.0f;
        [SerializeField, Tooltip("목표를 추적하는 속도")] 
        public float aimRotationSpeed = 30f;
        [SerializeField, Tooltip("목표물 알려주는 레이저")] 
        public LineRenderer aimLaser;
        
        [SerializeField, Tooltip("레이저 발사하는데 걸리는 시간")] 
        public float chargingTime = 3.0f;
        [SerializeField, Tooltip("발사 안내 마법진")]
        public GameObject chargingIndicator;
        [SerializeField, Tooltip("차징 이펙트")]
        public GameObject chargingEffect;

        [SerializeField, Tooltip("발사 이펙트")] 
        public Laser laserEffect;

        public AudioClip castingVoice;
        public AudioClip attackVoice;


        public float ChargingTime => chargingTime;
        public GameObject ChargingEffect => chargingEffect;
    }

    [System.Serializable]
    private class Phase1_Skill3Data
    {
        [SerializeField, Range(0, 1)]
        [Tooltip("유저 주변에 마법이 생길 확률\n")]
        public float shootToUserPercent = 0.3f;

        [SerializeField, Tooltip("유저 주변 판정 범위")]
        public float userRadius = 4f;

        [SerializeField, Tooltip("폭발 생기는 범위(사각 영역)")]
        public Vector2 explosionArea = new Vector2(12f, 13f);

        [SerializeField, Tooltip("보스 주변 제외할 범위")] 
        public float excludeRadius = 5f;

        [SerializeField, Tooltip("행동 지속 시간")]
        public float actionTime = 6f;

        [SerializeField, Tooltip("폭발 주기")]
        public float explosionInterval = 1f;

        [SerializeField, Tooltip("주기 당 폭발 횟수")]
        public float explosionCount = 3;

        [SerializeField, Tooltip("생성되는 마법")] 
        public DelayedMagic explosionMagicPrefab;

        public AudioClip skillVoice;
    }

    [System.Serializable]
    private class Phase1_Skill4Data
    {
        [SerializeField, Tooltip("기 모으는 이펙트")]
        public ParticleSystem chargingEffect;

        [SerializeField, Tooltip("밀치기 효과")]
        public Knockback knockbackMagicPrefab;

        public AudioClip skillVoice;
    }

    [System.Serializable]
    private class Phase1_Skill5Data
    {
        [SerializeField, Tooltip("기 모으는 시간")] 
        public ParticleSystem chargingEffect;

        [SerializeField, Tooltip("기 모으기 이펙트 구체")]
        public GameObject chargingSphere;

        [SerializeField, Tooltip("차지 시간")] 
        public float chargingTime = 5.0f;

        [SerializeField, Tooltip("차지 이펙트 스케일(최소, 최대)")]
        public Vector2 chargingScale = new Vector2(0.002f, 0.005f);

        [SerializeField, Tooltip("퍼지기 공격")] 
        public Knockback spreadPrefab;

        public AudioClip castingVoice;
        public AudioClip attackVoice;
    }

    [Header("Phase 1")]
    [SerializeField, Tooltip("Phase 1에 쓸 스킬 시간 정보")]
    private BossSkillData[] phase1SkillDatas = new BossSkillData[5];

    [SerializeField]
    private Phase1_Skill1Data phase1Skill1;
    [SerializeField]
    private Phase1_Skill2Data phase1Skill2;
    [SerializeField]
    private Phase1_Skill3Data phase1Skill3;
    [SerializeField]
    private Phase1_Skill4Data phase1Skill4;
    [SerializeField]
    private Phase1_Skill5Data phase1Skill5;

    private Coroutine actionDelayCoroutine = null;

    private static readonly int inChargeID = Animator.StringToHash("InCharge");


    #region Common Action
    // 참고 : StopAllCoroutine()을 하면 쿨타임 감소 코루틴도 멈출 수 있기 때문에 Update문에서 돌림
    private void DecreaseCooldown()
    {
        for (int i = 0; i < phase1SkillDatas.Length; i++)
        {
            phase1SkillDatas[i].CurrentCooldown -= Time.deltaTime;
        }
    }

    public void Phase1_JudgeAction()
    {
        // 플레이어 쪽으로 고개 돌리기
        useRotateToward = true;

        if (phase != 1)
        {
            return;
        }

        if (IsPlayerNearby() && phase1SkillDatas[3].CurrentCooldown <= 0)
        {
            Phase1_UseSkill(3);
            return;
        }

        // 발악 패턴 : 피 30% 이하면 발동
        float percent = (float)charStatus.CurrentHp / charStatus.maxHp;
        if (percent < 0.3f && phase1SkillDatas[4].CurrentCooldown <= 0)
        {
            Phase1_UseSkill(4);
            return;
        }

        int minCooldownNum = -1;
        float minCooldown = Single.MaxValue;
        for (int i = 0; i < phase1SkillDatas.Length; i++)
        {
            if (i == 3)
            {
                continue;
            }

            if (phase1SkillDatas[i].CurrentCooldown < minCooldown)
            {
                minCooldown = phase1SkillDatas[i].CurrentCooldown;
                minCooldownNum = i;
            }
        }

        // 모두 쿨다운이면 모든 스킬의 쿨다운을 전부 줄여놓는다
        if (minCooldown > 0)
        {
            for (int i = 0; i < phase1SkillDatas.Length; i++)
            {
                phase1SkillDatas[i].CurrentCooldown -= minCooldown;
            }
        }
        else
        {
            Phase1_UseSkill(minCooldownNum);
        }
    }

    private void Phase1_UseSkill(int skillNum)
    {
        var randomCooldown = UnityEngine.Random.Range(
            phase1SkillDatas[skillNum].Cooldown.x, phase1SkillDatas[skillNum].Cooldown.y);
        phase1SkillDatas[skillNum].CurrentCooldown = randomCooldown;
        animator.SetInteger(skillStateID, skillNum);
    }

    private bool IsPlayerNearby()
    {
        return Vector3.Distance(transform.position, attackTarget.position) < 3;
    }

    private IEnumerator IESkillCharging(float chargingTime, IEnumerator inChargeAction, IEnumerator endChargeAction)
    {
        animator.SetBool(inChargeID, true);
        if (inChargeAction != null)
        {
            StartCoroutine(nameof(inChargeAction));
        }
        yield return new WaitForSeconds(chargingTime);
        animator.SetBool(inChargeID, false);
        if (inChargeAction != null)
        {
            StopCoroutine(nameof(inChargeAction));
        }

        if (endChargeAction != null)
        {
           StartCoroutine(endChargeAction);
        }
    }

    private IEnumerator IESkillCharging(float chargingTime, Action inChargeAction, Action endChargeAction)
    {
        animator.SetBool(inChargeID, true);
        if (inChargeAction != null)
        {
            inChargeAction();
        }
        yield return new WaitForSeconds(chargingTime);
        animator.SetBool(inChargeID, false);

        if (endChargeAction != null)
        {
            endChargeAction();
        }
    }
    #endregion


    #region Skill 1

    private void Skill1_Init()
    {
        phase1Skill1.targetTr = GameManager.eye;
    }

    private void Skill1_Voice()
    {
        audioSource.PlayOneShot(phase1Skill1.skillVoice);
    }

    private IEnumerator IESkill1_Shoot()
    {
        var info = phase1Skill1;
        for (int i = 0; i < info.MagicMissileCount; i++)
        {
            var circle = Random.insideUnitCircle * info.radius;
            // 발사 위치를 기준으로 원형 범위로 랜덤하게 발사한다
            var magicPos = info.firePosition.position +
                           transform.right * circle.x + transform.up * circle.y;
            var magicRot = Quaternion.LookRotation((info.targetTr.position - magicPos).normalized);

            var magic = Instantiate(info.DelayedMagicPrefab, magicPos, magicRot);
            magic.StartMagic();
            yield return new WaitForSeconds(info.Interval);
        }
    }

    #endregion

    #region Skill 2

    private void InitSkill2()
    {
    }

    private IEnumerator IEPhase1_Skill2_Aim()
    {
        var info = phase1Skill2;

        print("In Pattern 2 Aim");
        animator.SetBool(inChargeID, true);
        audioSource.PlayOneShot(info.castingVoice);

        var laser = info.aimLaser;
        var laserTr = laser.transform;
        info.aimLaser.gameObject.SetActive(true);
        float currentTime = 0;
        while (currentTime < info.aimTime)
        {
            // 추적 중엔 더 빨리 돌아감
            currentRotationSpeed = info.aimRotationSpeed;

            // 레이저 타겟 설정
            laser.SetPosition(0, info.aimLaser.transform.position);
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Player");
            
            laserTr.forward = (attackTarget.transform.position - laserTr.position).normalized;
            if (Physics.Raycast(laserTr.position, laserTr.forward, out hit, 100, layerMask))
            {
                laser.SetPosition(1, hit.point);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
        laser.gameObject.SetActive(false);

        // 발사 시간
        useRotateToward = false;
        info.chargingEffect.gameObject.SetActive(true);
        info.chargingIndicator.SetActive(true);

        info.chargingIndicator.transform.localScale = Vector3.one;
        Sequence s = DOTween.Sequence();
        s.Append(info.chargingIndicator.transform.DOLocalRotate(new Vector3(0, 0, 1080f), info.chargingTime))
            .Join(info.chargingIndicator.transform.DOScale(0.3f, info.chargingTime));
        s.Play();
        yield return new WaitForSeconds(info.chargingTime);

        info.chargingIndicator.SetActive(false);
        info.chargingEffect.gameObject.SetActive(false);

        animator.SetBool(inChargeID, false);
    }

    private void Phase1_Skill2_ShootLaser()
    {
        audioSource.PlayOneShot(phase1Skill2.attackVoice);
        var laser = phase1Skill2.laserEffect;
        laser.gameObject.SetActive(true);
        var laserTr = laser.transform;

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Player");
        laserTr.forward = phase1Skill2.aimLaser.transform.forward;
        if (Physics.Raycast(laserTr.position, laserTr.forward, out hit, 100, layerMask))
        {
            laser.SetTargetPos(hit.point + laserTr.forward);
        }
    }
    #endregion

    #region Skill 3

    private void Skill3_Start()
    {
        audioSource.PlayOneShot(phase1Skill3.skillVoice);
    }


    private IEnumerator IESkill3_Casting()
    {
        animator.SetBool(inChargeID, true);
        StartCoroutine(nameof(IESkill3_Explosion));
        yield return new WaitForSeconds(phase1Skill3.actionTime);
        animator.SetBool(inChargeID, false);
        StopCoroutine(nameof(IESkill3_Explosion));
    }

    private IEnumerator IESkill3_Explosion()
    {
        var info = phase1Skill3;
        while (true)
        {
            // 폭발 생성
            for (int i = 0; i < info.explosionCount; i++)
            {
                Skill3_MakeExplosion();
            }
            yield return new WaitForSeconds(info.explosionInterval);
        }
    }

    private void Skill3_MakeExplosion()
    {
        var data = phase1Skill3;

        Vector2 posXZ;
        Vector3 origin;
        float chance = Random.Range(0f, 1f);
        if (chance < data.shootToUserPercent)
        {
            posXZ = Random.insideUnitCircle * data.userRadius;
            origin = attackTarget.position + new Vector3(posXZ.x, 0, posXZ.y);
        }
        else
        {
            int count = 0;
            do
            {
                // 무한 루프 방지
                Debug.Assert(++count < 10, "Error : make explosion entered abnormal loop count");

                float x = Random.Range(-data.explosionArea.x, -data.explosionArea.x);
                float z = Random.Range(-data.explosionArea.y, -data.explosionArea.y);
                posXZ = new Vector2(x, z);
            } while (posXZ.SqrMagnitude() < data.excludeRadius * data.excludeRadius);

            origin = transform.position + new Vector3(posXZ.x, 0, posXZ.y);
        }

        // 패턴은 지면에 적용되어야 함
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 100, 1 << LayerMask.NameToLayer("Default")))
        {
            var explosion = Instantiate(data.explosionMagicPrefab, hit.point, Quaternion.identity);
            explosion.StartMagic();
        }
    }

    #endregion

    #region Skill 4

    private void Skill4_Start()
    {
        audioSource.PlayOneShot(phase1Skill4.skillVoice);
        // 차지 이펙트
    }

    private void Skill4_Knockback()
    {
        var data = phase1Skill4;
        // 넉백 스킬 사용
        Instantiate(data.knockbackMagicPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
    #endregion

    #region Skill 5

    private void Skill5_Charging()
    {
        audioSource.PlayOneShot(phase1Skill5.castingVoice);
        charStatus.ShockResistPercent = 0;
        StartCoroutine(IESkillCharging(phase1Skill5.chargingTime, Skill5_EffectScaleUp, Skill5_EndCharge));
    }

    private void Skill5_EffectScaleUp()
    {
        var data = phase1Skill5;

        data.chargingEffect.gameObject.SetActive(true);
        data.chargingSphere.SetActive(true);
        data.chargingEffect.transform.localScale = Vector3.one * data.chargingScale.x;
        data.chargingEffect.transform.DOScale(data.chargingScale.y, data.chargingTime);
        data.chargingSphere.transform.localScale = Vector3.one * data.chargingScale.x * 5;
        data.chargingSphere.transform.DOScale(data.chargingScale.y * 5, data.chargingTime);
    }

    private void Skill5_EndCharge()
    {
        phase1Skill5.chargingEffect.gameObject.SetActive(false);
    }
    

    private void Skill5_Spread()
    {
        audioSource.PlayOneShot(phase1Skill5.attackVoice);
        phase1Skill5.chargingSphere.gameObject.SetActive(false);
        var skill = Instantiate(phase1Skill5.spreadPrefab, transform.position, Quaternion.identity);
    }

    #endregion

    private void Phase1_End()
    {
        // 다 캔슬하고 보스 쓰러짐 호출
        // 보스 피 회복
        charStatus.ResetStatus();

        // 페이즈 1 끝내기
        charStatus.onDead -= Phase1_End;


        // 페이즈 2 시작
    }
}
