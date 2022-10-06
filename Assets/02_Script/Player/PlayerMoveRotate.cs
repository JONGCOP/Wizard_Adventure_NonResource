using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 플레이어 이동 및 회전 관련 로직을 처리
/// </summary>
public class PlayerMoveRotate : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField, Tooltip("중력 설정값")]
    private float gravity = 9.81f;
    private float yVelocity = 0;

    // Rotation
    [SerializeField, Tooltip("한 번 회전하는 양 (X, Y)")] 
    private Vector2 rotationScale = new Vector2(15.0f, 45.0f);
    private int previousAxisX = 0;
    private int previousAxisY = 0;

    private Vector2Int previousRotDir = Vector2Int.zero;

    [Header("Teleport")] 
    [SerializeField] private float teleportRange = 7.0f;
    [SerializeField] private float controlPointHeightMultiplier = 0.25f;
    [SerializeField] private Transform teleportDirectionTransform;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private Transform footPos;
    [SerializeField] private float dashTime = 0.25f;

    private bool isTeleporting = false;

    private int mapLayerMask;
    private int enemyLayerMask;

    [Header("Hand Control")] 
    [SerializeField, Tooltip("왼손 컨트롤러")]
    private HandController leftHandController;

    [Header("OVRCameraRig")]
    private Transform m_CameraRig;
    private Transform m_CenterEyeAnchor;
    public OVRCameraRig m_OVRCameraRig;
    private OVRManager ovrManager;

    [Header("Sounds")] 
    [SerializeField] [NotNull]
    private AudioSource walkSoundSource;
    [SerializeField]
    private AudioClip dashSound;

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        mapLayerMask = 1 << LayerMask.NameToLayer("Default");
        enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    void Start()
    {
        m_CenterEyeAnchor = m_OVRCameraRig.centerEyeAnchor;
        m_CameraRig = m_OVRCameraRig.transform;
        ovrManager = m_OVRCameraRig.GetComponent<OVRManager>();

        //m_CameraRig.eulerAngles = new Vector3(0, -m_CenterEyeAnchor.transform.eulerAngles.y, 0);  
    }

    private void Update()
    {
        ApplyGravity();
    }

    //Do the same as OVRManager.display.RecenterPose() but works in Virtual Desktop and EyeLevelTracking
    public void ResetVRPosition(Transform teleportPoint) 
    {
        float currentRotY = m_CenterEyeAnchor.eulerAngles.y;
        float targetYRotation = 0.0f;
        float difference = targetYRotation - currentRotY;
        m_CameraRig.Rotate(0, difference, 0);

        Vector3 newPos = new Vector3(teleportPoint.position.x - m_CenterEyeAnchor.position.x, 0, teleportPoint.position.z - m_CenterEyeAnchor.position.z);
        m_CameraRig.position += newPos;
    }

    public void ResetVRRotate()
    {
        float currentRotY = m_CenterEyeAnchor.eulerAngles.y;
        float targetYRotation = 0.0f;
        float difference = targetYRotation - currentRotY;
        ovrManager.headPoseRelativeOffsetRotation = m_CenterEyeAnchor.localEulerAngles;

        m_CameraRig.Rotate(0, difference, 0);
    }

    private void ApplyGravity()
    {
        if (cc.collisionFlags == CollisionFlags.Below)
        {
            yVelocity = 0;
        }

        yVelocity -= gravity * Time.deltaTime;
        if (cc.enabled)
        {
            cc.Move(new Vector3(0, yVelocity, 0));
        }
    }

    /// <summary>
    /// Player의 위치를 강제로 변경한다
    /// - Player의 위치를 transform으로 변경하면 Character Controller로 인해 이동되지 않음
    /// - 이 함수는 위치 이동 시 cc등을 조작하여 강제로 해당 위치에 있도록 만듦
    /// </summary>
    public void SetPos(Vector3 targetPos, Vector3 direction)
    {
        cc.enabled = false;
        yVelocity = 0;
        // 발 위치 보정
        transform.position = targetPos - footPos.localPosition;
        transform.forward = direction;
        cc.enabled = true;
    }

    /// <summary>
    /// 텔레포트(대쉬) 기반 이동. 
    /// </summary>
    public void ToDash(Vector3 targetPos, float time)
    {
        StopAllCoroutines();
        StartCoroutine(IEDash(targetPos, time));
    }

    /// <summary>
    /// cc 기반 이동. 걷기 연출 시 사용
    /// </summary>
    public void ToMove(Vector3 targetPos, float time)
    {
        StopAllCoroutines();
        StartCoroutine(IEToMove(targetPos, time));
    }

    private IEnumerator IEToMove(Vector3 targetPos, float time)
    {
        walkSoundSource.Play();

        cc.enabled = true;
        isTeleporting = true;

        print("Player Moving");
        while (Vector3.Distance(transform.position, new Vector3(targetPos.x, transform.position.y, targetPos.z)) > 2f &&
            time > 0)
        {
            var direction = targetPos - transform.position;
            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            var move = new Vector3(direction.x, 0, direction.y).normalized;
            cc.Move(move * scaledMoveSpeed);
            time -= Time.deltaTime;

            yield return null;
        }
        isTeleporting = false;

        // 대상 위치로 Lerp 보정
        targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        yield return IEDash(targetPos, 0.3f);

        walkSoundSource.Stop();
    }

    public void StartTeleport()
    {
        leftHandController.SetLeftHandAction(HandController.LeftAction.Teleport);
        line.gameObject.SetActive(true);
        teleportTarget.gameObject.SetActive(true);
    }

    public void OnTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(teleportDirectionTransform.position, teleportDirectionTransform.forward, out hit,
                teleportRange, mapLayerMask | enemyLayerMask))
        {
            //Vector3 teleportPos;
            //int hitLayerMask = 1 << hit.collider.gameObject.layer;
            //// 몬스터와 충돌하는 경우 혹은 벽에 텔레포트하는 경우 충돌점 앞에 이동하도록 보정
            //if ((hitLayerMask & enemyLayerMask) > 0 || 
            //    Vector3.Dot(hit.normal, Vector3.up) < 0.5f)
            //{
            //    RaycastHit hit2;
            //    Vector3 revisedPos = hit.point + hit.normal;
            //    bool canRevise = Physics.Raycast(revisedPos, Vector3.down, out hit2, Single.PositiveInfinity, mapLayerMask);
            //    // Z-fighting이 일어나지 않게 텔레포트 위치 보정
            //    teleportPos = hit2.point + Vector3.up * 0.05f;
            //}
            //// 그렇지 않다면 이동 보정을 하지 않음
            //else
            //{
            //    // Z-fighting이 일어나지 않게 텔레포트 위치 보정
            //    teleportPos = hit.point + Vector3.up * 0.05f;
            //}
            //teleportTarget.position = teleportPos;
            //DrawTeleportLineCurve(teleportDirectionTransform.position, teleportTarget.position);
            ToTeleport(hit);
        }
        else
        {
            Vector3 maxRangePos = teleportDirectionTransform.position + 
                teleportRange * teleportDirectionTransform.forward;
            if (Physics.Raycast(maxRangePos, Vector3.down, out hit, 100, mapLayerMask | enemyLayerMask))
            {
                ToTeleport(hit);
            }
        }
    }

    private void ToTeleport(RaycastHit hit)
    {
        Vector3 teleportPos;
        int hitLayerMask = 1 << hit.collider.gameObject.layer;
        // 몬스터와 충돌하는 경우 혹은 벽에 텔레포트하는 경우 충돌점 앞에 이동하도록 보정
        if ((hitLayerMask & enemyLayerMask) > 0 ||
            Vector3.Dot(hit.normal, Vector3.up) < 0.5f)
        {
            RaycastHit hit2;
            Vector3 revisedPos = hit.point + hit.normal;
            bool canRevise = Physics.Raycast(revisedPos, Vector3.down, out hit2, Single.PositiveInfinity, mapLayerMask);
            // Z-fighting이 일어나지 않게 텔레포트 위치 보정
            teleportPos = hit2.point + Vector3.up * 0.05f;
        }
        // 그렇지 않다면 이동 보정을 하지 않음
        else
        {
            // Z-fighting이 일어나지 않게 텔레포트 위치 보정
            teleportPos = hit.point + Vector3.up * 0.05f;
        }
        teleportTarget.position = teleportPos;
        DrawTeleportLineCurve(teleportDirectionTransform.position, teleportTarget.position);
    }

    public void EndTeleport()
    {
        leftHandController.SetLeftHandAction(HandController.LeftAction.Default);
        line.gameObject.SetActive(false);
        teleportTarget.gameObject.SetActive(false);

        if (!isTeleporting)
        {
            SFXPlayer.Instance.PlaySpatialSound(transform.position, dashSound);
            StartCoroutine(IEDash(teleportTarget.position, dashTime));
        }
    }

    private IEnumerator IEDash(Vector3 endPosition, float time)
    {
        // 텔레포트 동안은 다시 텔레포트할 수 없다
        isTeleporting = true;
        cc.enabled = false;
        Vector3 origin = transform.position;
        Vector3 targetPos = endPosition - footPos.localPosition; // 발 위치 보정

        // 대쉬 하는 동안 Vignette 효과 적용

        float multiplier = 1 / time;
        for (float t = 0.0f; t < time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(origin, targetPos, t * multiplier);
            yield return null;
        }
        transform.position = targetPos;

        cc.enabled = true;
        isTeleporting = false;
    }

    private void DrawTeleportLineCurve(Vector3 startPos, Vector3 endPos)
    {
        Vector3 mid = (startPos + endPos) * 0.5f;
        Vector3 controlPointPos = mid + Vector3.up * Vector3.Distance(startPos, endPos) * controlPointHeightMultiplier;

        line.SetPosition(0, startPos);
        for (int i = 1; i < line.positionCount - 1; i++)
        {
            float t = (float)i / line.positionCount;
            var m = Vector3.Lerp(startPos, controlPointPos, t);
            var n = Vector3.Lerp(controlPointPos, endPos, t);
            var b = Vector3.Lerp(m, n, t);

            line.SetPosition(i, b);
        }
        line.SetPosition(line.positionCount - 1, endPos);
    }

    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01) return;

        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        // For simplicity's sake, we just keep movement in a single plane here.
        // Rotate direction according to world Y rotation of player.
        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        if (cc.enabled)
        {
            cc.Move(move * scaledMoveSpeed);
        }
    }

    /// <summary>
    /// 플레이어를 X축으로 회전 (PC 디버그용)
    /// </summary>
    public void RotateX(int axisRaw)
    {
        var currentAxisRaw = Mathf.RoundToInt(axisRaw);
        if (currentAxisRaw == previousAxisY)
        {
            return;
        }

        // 회전 적용
        var localAngles = transform.localEulerAngles;
        localAngles += new Vector3(-currentAxisRaw * rotationScale.x, 0, 0);
        transform.localEulerAngles = localAngles;

        previousAxisY = currentAxisRaw;
    }

    /// <summary>
    /// 플레이어를 Y축으로 회전
    /// </summary>
    public void RotateY(int axisRaw)
    {
        var currentAxisRaw = Mathf.RoundToInt(axisRaw);
        if (currentAxisRaw == previousAxisX)
        {
            return;
        }

        // 회전 적용
        var localAngles = transform.localEulerAngles;
        localAngles += new Vector3(0, currentAxisRaw * rotationScale.y, 0);
        transform.localEulerAngles = localAngles;

        previousAxisX = currentAxisRaw;
    }

    public void Knockback(Vector3 knockbackVelocity)
    {
        yVelocity = knockbackVelocity.y;
        StartCoroutine(IEKnockback(new Vector3(knockbackVelocity.x, 0, knockbackVelocity.z)));
    }

    private IEnumerator IEKnockback(Vector3 knockbackVecXZ)
    {
        float speed = knockbackVecXZ.magnitude;
        Vector3 direction = knockbackVecXZ.normalized;
        float decreaseSpeed = 5f;

        // 넉백 중엔 텔레포트 막아야함
        isTeleporting = true;
        while (speed > 0.01f)
        {
            cc.Move(direction * (speed * Time.deltaTime));
            speed -= Time.deltaTime * decreaseSpeed;
            yield return null;
        }

        isTeleporting = false;
    }
}
