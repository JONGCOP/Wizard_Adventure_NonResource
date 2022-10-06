using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ���� �ð����� ���� �� ����� �����ϴ� ����
/// �ۼ��� - ����ö
/// </summary>
public class ThunderCloud : Magic
{
    [SerializeField, Tooltip("���� �ִ� ����")]
    private float maxHeight = 15;

    [SerializeField, Tooltip("������ õ�忡 ���� �� �� ���� ������ ����")]
    private float downHeight = 2f;

    [SerializeField, Tooltip("���� ���� ũ��")]
    private float circleSize = 10;

    [SerializeField, Tooltip("���� ���� Ƚ��")] 
    private int attackCount = 10;

    [SerializeField, Tooltip("���� ���� �ֱ�")] 
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

    // õ�� ���� ��ġ�� �����ǵ��� �����Ѵ�
    // ���� õ���� �ʹ� ���ٸ� �ִ� ���̿� �����ǵ��� �����Ѵ�
    private void RevisePosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit,
                maxHeight, 1 << LayerMask.NameToLayer("Default")))
        {
            // õ�� ���� �Ʒ� 2m ��ġ�� �����ǰ� ����
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

    // ���� ���� ���� ���� �� ��ġ�� �����Ǽ� ���� ���� �õ�
    private void Thunder()
    {
        var circlePos = Random.insideUnitCircle * circleSize;
        Vector3 startPos = new Vector3(transform.position.x + circlePos.x, transform.position.y, transform.position.z + circlePos.y);

        // Flare ������ ��ġ ���� �� ����
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
