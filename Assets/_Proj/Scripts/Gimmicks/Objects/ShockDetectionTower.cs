using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ShockDetectionTower : MonoBehaviour
{
    [Header("Relay")]
    [Tooltip("이웃한 탑을 찾을 반경(월드 단위)")] public float relayRadius = 4f;
    [Tooltip("이 값 미만이면 전이 중단")] public float minRelayStrength = 0.1f;
    [Tooltip("전이 지연(초)")] public float relayDelay = 0.05f;
    [Tooltip("토큰별 로컬 쿨다운(초)")] public float localCooldown = 0.15f;

    [Header("Occlusion")]
    public bool useOcclusion = true;
    public LayerMask occluderMask;

    [Header("Layers")]
    [Tooltip("이웃 감지탑 레이어")] public LayerMask towerLayer;
    [Tooltip("Door 레이어")] public LayerMask doorLayer;

    [Header("Events")]
    [Tooltip("탑이 최초로 신호를 받았을 때 발생")] public UnityEvent onShock;

    // token -> last time
    private readonly Dictionary<long, float> seen = new();

    //ShockPing 등에서 호출. 같은 token은 쿨다운 내 중복 무시
    public void ReceiveShock(long token, float strength, Vector3 origin, int hop)
    {
        Debug.Log($"[Tower] 타워 충격파 감지", this);

        float now = Time.time;
        if (seen.TryGetValue(token, out float t) && now - t < localCooldown) return;
        seen[token] = now;

        // 로컬 이벤트
        onShock?.Invoke();

        // 반경 내 ISignalSender 전송(문 트리거는 동료 Sender가 책임)
        TransmitToDoor();

        // 이웃에게 릴레이(전이)
        if (strength < minRelayStrength) return;
        StartCoroutine(RelayAfterDelay(token, strength, hop + 1, origin));
    }

    private void TransmitToDoor()
    {
        var cols = Physics.OverlapSphere(transform.position, relayRadius, doorLayer, QueryTriggerInteraction.Ignore);

        foreach(var c in cols)
        {
            var sender = c.GetComponentInChildren<ISignalSender>();
            if (sender == null) continue;
            try { sender.SendSignal(); }
            catch { /* sender 구현 쪽에서 처리 */}
        }
    }

    private IEnumerator RelayAfterDelay(long token, float nextStrength, int nextHop, Vector3 origin)
    {
        if (relayDelay > 0f) yield return new WaitForSeconds(relayDelay);

        var cols = Physics.OverlapSphere(transform.position, relayRadius, towerLayer, QueryTriggerInteraction.Ignore);
        if (cols == null || cols.Length == 0) yield break;

        foreach (var c in cols)
        {
            if (c.transform == transform) continue;
            var tower = c.GetComponent<ShockDetectionTower>();
            if (!tower) continue;

            if (useOcclusion)
            {
                Vector3 p0 = transform.position + Vector3.up * 0.5f;
                Vector3 p1 = tower.transform.position + Vector3.up * 0.5f;
                Vector3 dir = p1 - p0; float dist = dir.magnitude;
                if (dist > 0.01f && Physics.Raycast(p0, dir.normalized, dist - 0.05f, occluderMask, QueryTriggerInteraction.Ignore))
                    continue;
            }

            tower.ReceiveShock(token, nextStrength, transform.position, nextHop);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, relayRadius);
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, relayRadius);
    }
#endif
}
