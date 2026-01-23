using System.Collections;
using UnityEngine;

// PushableOrb(IronBall)는 낙하 착지 시 Shockwave를 발생시키는 특수 Pushable 오브젝트.
// 수직 낙하로 착지했을 때만 충격파를 발생시킴

[RequireComponent(typeof(Shockwave))]
public class PushableOrb : PushableObjects
{
    [SerializeField] private Shockwave shockwave;
    [SerializeField] private ShockPing shockPing;

    [Tooltip("Orb 자신이 충격파 발생시킬 수 있는 쿨타임")]
    public float orbCoolTime = 6f;
    private float lastShockwaveTime = -float.MaxValue;

    [Header("Orb Fall Detection")]
    public float probeUp = 0.1f;
    [Min(0.6f)] public float probeDown = 0.6f;
    private bool wasGrounded;
    [Tooltip("SphereCast 반지름 (Orb의 반지름)")]
    public float sphereRadius = 0.35f;
    public LayerMask orbLandLayer;

    private RingRange ring;
    [Tooltip("범위 시각화 ring 표시 시간")]
    [Range(0.5f, 2)] public float ringDuration;

    protected override void Awake()
    {
        base.Awake();
        allowSlope = false;
        shockwave = GetComponent<Shockwave>();
        shockPing = GetComponent<ShockPing>();
        wasGrounded = true;
        ring = GetComponentInChildren<RingRange>();
        ring.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * probeUp * tileSize;
        float distance = probeDown * tileSize;

        bool grounded = Physics.SphereCast(
            origin: transform.position + transform.up * probeUp * tileSize,
            sphereRadius,
            Vector3.down,
            out RaycastHit hit,
            maxDistance: probeDown * tileSize,
            layerMask: orbLandLayer,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );

        // 이전에는 공중이었는데, 지금은 지상에 닿은 경우 = 착지 완료
        if (!wasGrounded && grounded)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Ironball")) AudioEvents.Raise(SFXKey.InGameObject, 4, pooled: true, pos: gameObject.transform.position);
            
            TryFireShockwave(); // 충격파 발생 시도
        }
        wasGrounded = grounded;
    }

    // 중복 충격파 발생을 방지하기 위한 외부에 의한 충격파 리프트 면역 메서드(쿨타임)
    protected override bool IsImmuneToWaveLift()
    {
        return Time.time < lastShockwaveTime + orbCoolTime;
    }

    protected override void OnLanded()
    {
        isMoving = false;
        isFalling = false;

        if (Time.time - lastShockwaveTime < orbCoolTime)
            return;

        bool grounded = Physics.SphereCast(
            origin: transform.position + transform.up * probeUp * tileSize,
            sphereRadius,
            Vector3.down,
            out RaycastHit hit,
            maxDistance: probeDown * tileSize,
            layerMask: groundMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );
        if (!grounded || isMoving || isFalling)
            return;

        TryFireShockwave();
    }

    /// <summary>
    /// 충격파 발생 및 주변 객체 통지
    /// </summary>
    void TryFireShockwave()
    {
        if (shockwave == null) return;

        float now = Time.time;

        if (now - lastShockwaveTime < orbCoolTime) return;

        lastShockwaveTime = now;

        shockwave.Fire(
            origin: transform.position,
            tile: tileSize,
            radiusTiles: shockwave.radiusShock,
            targetLayer: shockwave.targetLayer,
            useOcclusion: shockwave.useOcclusion,
            occludeMask: shockwave.occluderMask,
            riseSeconds: shockwave.riseSec,
            hangSeconds: shockwave.hangSec,
            fallSeconds: shockwave.fallSec
        );

        StartCoroutine(ShowRingRange());

        // 주변 감지탑에 통지
        if (shockPing)
        {
            shockPing.PingTowers(transform.position);
        }
    }

    IEnumerator ShowRingRange()
    {
        ring.gameObject.SetActive(true);
        yield return new WaitForSeconds(ringDuration);
        ring.gameObject.SetActive(false);
    }

    // Orb(Ironball)는 근처 물체가 lifting 중이더라도 그 밑으로 밀릴 수 없음
    protected override bool CheckBlocking(Vector3 target)
    {
        var b = boxCol.bounds;
        Vector3 half = b.extents - Vector3.one * 0.005f;
        Vector3 center = new Vector3(target.x, target.y + b.extents.y, target.z);

        // 규칙상 차단(blocking)
        if (Physics.CheckBox(center, half, transform.rotation, blockingMask, QueryTriggerInteraction.Ignore))
            return true;

        // 점유 차단(허용 레이어 제외)
        var hits = Physics.OverlapBox(center, half, transform.rotation, ~throughLayer, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
        {
            if (rb && c.attachedRigidbody == rb) continue; // 자기 자신
            if (c.transform.IsChildOf(transform)) continue; // 자식
            return true;
        }
        return false;
    }
}
