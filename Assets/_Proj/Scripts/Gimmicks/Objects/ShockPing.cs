using UnityEngine;

public class ShockPing : MonoBehaviour
{
    [Header("같은 오브젝트의 Shockwave")]
    public Shockwave shockwave;

    [Header("감지탑 레이어")]
    public LayerMask towerLayer;
    public bool useOcclusion = false;
    [Tooltip("벽/지형 레이어")]
    public LayerMask occludeLayer;

    private static long _seed = 1;
    static long NewToken() => System.Threading.Interlocked.Increment(ref _seed);

    void Awake()
    {
        if (!shockwave) shockwave = GetComponent<Shockwave>();
    }

    //Shockwave 원점 기준 반경 내 탑들에 신호 토큰을 보냄
    public void PingTowers(Vector3 origin)
    {
        if (!shockwave) shockwave = GetComponent<Shockwave>();
        if (!shockwave) return;

        float rW = Mathf.Max(0.0001f, shockwave.radiusShock) * Mathf.Max(0.0001f, shockwave.tileHeight);

        var candidates = Physics.OverlapSphere(origin, rW, ~0, QueryTriggerInteraction.Ignore);
        int sent = 0;

        foreach (var c in candidates)
        {
            var tower = c.GetComponentInParent<ShockDetectionTower>();
            if (!tower) continue;

            // towerLayer에 포함된 루트 레이어만 통과
            if (((1 << tower.gameObject.layer) & towerLayer.value) == 0) continue;

            if (useOcclusion)
            {
                Vector3 p0 = origin + Vector3.up * 0.1f;
                Vector3 p1 = tower.transform.position + Vector3.up * 0.5f;
                Vector3 dir = p1 - p0; float dist = dir.magnitude;
                if (dist > 0.01f && Physics.Raycast(p0, dir.normalized, dist - 0.05f, occludeLayer, QueryTriggerInteraction.Ignore))
                    continue;
            }

            long token = System.Threading.Interlocked.Increment(ref _seed);
            tower.ReceiveShock(token, 1f, origin, 0);
            sent++;
        }

        Debug.Log($"[Ping] candidates={candidates.Length}, sentToTowers={sent}, rW={rW:F2}", this);
    }
}
