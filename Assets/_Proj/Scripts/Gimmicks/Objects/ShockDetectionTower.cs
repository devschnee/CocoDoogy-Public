using System.Collections;
using UnityEngine;

/// <summary>
/// Shockwave 발생을 감지하여
/// 연결된 수신기(Door)에 신호를 전달하고
/// 인근 Tower로 충격을 릴레이 하는 환경 반응 오브젝트
/// </summary>
[DisallowMultipleComponent]
public class ShockDetectionTower : MonoBehaviour, ISignalSender, ISignalReceiver
{
    [Header("Relay Settings")]
    public float relayRadius = 4f;
    public float relayDelay = 1f;
    public float towerCooldown = 10f;

    [Header("Occlusion")]
    public bool useOcclusion = true;
    public LayerMask occluderMask;
    [Tooltip("신호를 전송시킬 레이어. Door레이어도 포함돼야 함")]
    public LayerMask towerLayer;

    public bool IsOn { get; set; } // 그냥 ISignalReceiver 구현용

    // --- ISignalSender ---
    public ISignalReceiver Receiver { get; set; }

    private bool isCooling = false;


    void Start()
    {
        TryAutoConnect();
    }

    // 초기화 시 주변 오브젝트 중 ISignalReceiver를 자동 탐색하여 연결
    // 수동 연결이 없는 경우의 편의 기능. 기본적으로는 Map Editor에서 감지탑-문 쌍으로 지정 연결된 상태임.
    void TryAutoConnect()
    {
        if (Receiver != null) return;

        var cols = Physics.OverlapSphere(transform.position, relayRadius, ~0);
        foreach (var c in cols)
        {
            var receiver = c.GetComponentInParent<ISignalReceiver>();
            if (receiver != null)
            {
                Receiver = receiver;
                break;
            }
        }
    }

    // 충격파 수신 및 인근 Tower로 충격 릴레이
    public void ReceiveShock(Vector3 origin)
    {
        if (isCooling) { return; }

        GetComponent<ISignalSender>().SendSignal();
        
        // 쿨타임 진입
        StartCoroutine(CooldownTimer());

        // 주변 탑에 릴레이(전이)
        StartCoroutine(RelayToNearbyTowers(origin));
    }

    // 동일 Tower에서 연속 신호 전송을 방지하기 위한 쿨타임 코루틴
    private IEnumerator CooldownTimer()
    {
        isCooling = true;
        yield return new WaitForSeconds(towerCooldown);
        isCooling = false;
    }

    // 일정 시간 지연 후 반경 내 다른 Tower로 충격파 전달
    // 차폐가 활성화된 경우 Raycast로 시야 차단 검사
    private IEnumerator RelayToNearbyTowers(Vector3 origin)
    {
        yield return new WaitForSeconds(relayDelay);

        var cols = Physics.OverlapSphere(transform.position, relayRadius, towerLayer, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            if (c.transform == transform) continue;
            var tower = c.GetComponent<ShockDetectionTower>();
            if (!tower || tower.isCooling) continue;

            // 차폐 검사
            if (useOcclusion)
            {
                Vector3 p0 = transform.position + Vector3.up * 0.5f;
                Vector3 p1 = tower.transform.position + Vector3.up * 0.5f;
                Vector3 dir = p1 - p0; float dist = dir.magnitude;
                if (dist > 0.1f && Physics.Raycast(p0, dir.normalized, dist - 0.05f, occluderMask))
                    continue;
            }

            tower.ReceiveShock(transform.position);
        }
    }

    // --- ISignalSender ---
    public void SendSignal()
    {
        if (Receiver != null)
        {
            if (Receiver is DoorBlock door)
            {
                //door.OpenPermanently();
                // NOTE : ▼ 12/01 기획팀 요청(기획 변경)으로 Tower도 Switch와 같이 일반적인 신호 받도록 변경. 만약 기획이 원래대로 변경된다면 이 라인을 주석처리 하고 윗 라인을 주석 해제해주면 됨.
                Receiver.ReceiveSignal(); 
                Debug.Log($"[Tower] {name}: 문에 신호 전송(영구 열림) 완료");
            }
            else
            {
                // 일반적인 신호 전송
                Receiver.ReceiveSignal();
                Debug.Log($"[Tower] {name}: 수신기({Receiver.GetType().Name})에 신호 전송 완료");
            }
        }
        else
        {
            Debug.LogWarning($"[Tower] {name}: 연결된 수신기가 없음");
        }
    }

    // 다른 Tower로부터 전달된 신호를 Shock로 재처리
    public void ReceiveSignal()
    {
        // 다른 Tower가 나한테 신호 보낼 때
        ReceiveShock(Vector3.zero);
    }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, relayRadius);
    }
#endif
}