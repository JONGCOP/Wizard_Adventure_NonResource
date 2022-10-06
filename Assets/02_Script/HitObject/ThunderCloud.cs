using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 일정 시간마다 범위 내 대상을 공격하는 마법
/// 작성자 - 차영철
/// </summary>
public class ThunderCloud : Magic
{
    [SerializeField, Tooltip("구름 최대 높이")]
    private float maxHeight = 15;

    [SerializeField, Tooltip("구름이 천장에 닿을 때 더 낮게 설정할 높이")]
    private float downHeight = 2f;

    [SerializeField, Tooltip("구름 생성 크기")]
    private float circleSize = 10;

    [SerializeField, Tooltip("번개 공격 횟수")] 
    private int attackCount = 10;

    [SerializeField, Tooltip("번개 공격 주기")] 
    private float attackInterval = 0.25f;

    [SerializeField] 
    private ParticleSystem[] flareParticles;
    private Queue<ParticleSystem> flareParticlesQueue = new Queue<ParticleSystem>();

    [SerializeField] 
    private LightningBolt[] lightningBolts;
    private Queue<LightningBolt> lightningBoltQueue = new Queue<LightningBolt>();

    private void Awake()
    {
        foreach (var flareParticle in flareParticles)
        {
            flareParticlesQueue.Enqueue(flareParticle);
        }
        foreach (var lightningBolt in lightningBolts)
        {
            lightningBoltQueue.Enqueue(lightningBolt);
        }
    }

    private void OnEnable()
    {
        foreach (var flareParticle in flareParticles)
        {
            flareParticle.gameObject.SetActive(false);
        }
        foreach (var lightningBolt in lightningBolts)
        {
            lightningBolt.gameObject.SetActive(false);
        }
    }

    public override void StartMagic()
    {
        StartCoroutine(IEThunderAttack());
        RevisePosition();
    }

    // 천장 이전 위치에 생성되도록 보정한다
    // 만약 천장이 너무 높다면 최대 높이에 생성되도록 설정한다
    private void RevisePosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit,
                maxHeight, 1 << LayerMask.NameToLayer("Default")))
        {
            // 천장 기준 아래 2m 위치에 생성되게 보정
            transform.position = hit.point + Vector3.down * downHeight;
        }
        else
        {
            print("can't revise position");
            transform.position += Vector3.up * maxHeight;
        }
    }

    private IEnumerator IEThunderAttack()
    {
        float waitTime = 0.25f;
        for (int i = 0; i < attackCount; i++)
        {
            Thunder();
            yield return new WaitForSeconds(waitTime);
        }

        gameObject.SetActive(false);
    }

    // 번개 시작 점이 랜덤 원 위치에 생성되서 적을 공격 시도
    private void Thunder()
    {
        var circlePos = Random.insideUnitCircle * circleSize;
        Vector3 startPos = new Vector3(transform.position.x + circlePos.x, transform.position.y, transform.position.z + circlePos.y);

        // Flare 꺼내서 위치 조정 후 실행
        var flare = flareParticlesQueue.Dequeue();
        flareParticlesQueue.Enqueue(flare);
        flare.gameObject.SetActive(true);
        flare.transform.position = startPos + Vector3.down;

        var lightning = lightningBoltQueue.Dequeue();
        lightningBoltQueue.Enqueue(lightning);
        lightning.gameObject.SetActive(true);
        lightning.SetPosition(startPos);
        lightning.SetDirection(Vector3.down);
        lightning.StartMagic();
    }
}
