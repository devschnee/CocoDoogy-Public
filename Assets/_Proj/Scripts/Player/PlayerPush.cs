using UnityEngine;

/// <summary>
/// 플레이어 이동 입력을 기반으로
/// 전방의 Pushable 오브젝트와 상호작용(밀기)을 시도하는 이동 전략 클래스
/// (실제 이동/차단/낙하 규칙은 PushableObject가 담당. 본 클래스는 입력 안정화와 대상 선정까지만 책임짐)
/// 연속 푸시 방지를 위해 쿨타임 기반 제어를 포함함
/// </summary>

public class PlayerPush : MonoBehaviour, IMoveStrategy
{
    [Header("Push Settings")]
    public float tileSize = 1f;
    public LayerMask pushables;
    [Tooltip("얼마나 가까워야 밀기 시작할지 판단. 값이 작을수록 더 오래 미는 것 처럼 보임")]
    public float frontOffset = 0.4f;

    [Header("PushDelay")]
    [Range(0.1f, 0.7f)] public float pushCooltime = 0.3f; // push 종료 후 딜레이
    private float currCooltime = 0f; // 현재 쿨타임 계산

    // 현재 밀고 있는 대상 추적을 위한
    private IPushHandler currPushHandler = null;

    public bool isPushing => currPushHandler != null;
    private Vector3 lastDirN = Vector3.forward; // 기즈모용 변수

    public (Vector3, Vector3) Execute(Vector3 moveDir, Rigidbody rb, PlayerMovement player)
    {
        // 밀기 동작 직후 짧은 쿨타임을 주어 의도치 않은 연속 밀기 방지
        if (currCooltime > 0f)
        {
            currCooltime -= Time.deltaTime;

            // 쿨타임 중에는 이동 속도 감속
            Vector3 moveDelay = moveDir * 0.45f;
            return (moveDelay, Vector3.zero);
        }

        // 입력 없으면 즉시 리셋
        if (moveDir.sqrMagnitude < 1e-6f)
        {
            if (currPushHandler != null)
            {
                currPushHandler.StopPushAttempt();
                currPushHandler = null;
            }
            return (Vector3.zero, Vector3.zero);
        }

        // 입력을 4방향으로 스냅하여
        // 조이스틱의 미세한 각도 떨림으로 인한 홀드-리셋 반복 방지
        Vector2Int dir4 = player.To4Dir(moveDir); // up/right/left/down 중 하나로 스냅
        Vector3 dirCard = new Vector3(dir4.x, 0f, dir4.y); // 이걸로 캐스트/푸시 둘 다 수행
        Vector3 dirN = dirCard; // 이미 정규화됨 (x/z는 -1,0,1이라서)
        lastDirN = dirN; // 기즈모용

        // 앞 1칸 두께 있게 훑기 (레이어 제한 없이 -> IPushHandler로 필터)
        Vector3 halfExtents = new(.2f, .4f, .2f);
        
        // frontOffset은 감지 거리 조절용 파라미터로도 사용되어 가변적이므로
        // Max 계산 값으로 1번 인자를 갖기 위해서 0.9f로 변경
        // front를 앞으로 뺀 이유는 수직 검사를 하되 내 머리 위는 검사대상에서 빼기 위해서임
        float front = Mathf.Max(0.9f, frontOffset); 

        // front를 전방으로 이동시킨 만큼 실제 검사 거리를 줄여 원격 푸시를 방지
        float maxDist = tileSize * .8f;

        Vector3 origin = rb.position + Vector3.up * .8f + dirN * front;

        //SphereCastAll에서 BoxCastAll로 변경합니다.
        var hits = Physics.BoxCastAll(
            origin,
            halfExtents,
            dirN,
            Quaternion.identity,
            maxDist,
            pushables, // 레이어 전부 허용. 실제 푸시 가능 여부는 IPushHandler로 필터링
            QueryTriggerInteraction.Ignore
        );

        //레이어를 전부 허용했기때문에, 뭐든지 다 검출되게 됨. 여기서 거리순으로 정렬
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        IPushHandler next = null;
        foreach (var h in hits)
        {
            if (h.rigidbody && h.rigidbody == rb) continue; // 자기 자신 무시
            if (h.collider.TryGetComponent<IPushHandler>(out var handler))
            {

                float dist = Vector3.Distance(h.collider.transform.position, transform.position);
                // 여러 IPushHandler가 동시에 검출되면 원격 or 모호한 push로 판단하여 무효 처리
                if (next != null) { next = null; break; }
                next = (next == null && Mathf.Approximately(h.distance,0f)) ? handler : null;
            }
            //내 입력 기준 가까운 오브젝트 순으로 검사하다가 밀 수 있는 오브젝트가 아니면 즉시 무시(원격 푸시 막는 처리)
            else
            {
                return (moveDir, Vector3.zero);
            }
        }

        // 대상 처리 (방향 고정값 dir4로만 시도 -> 흔들려도 누적 유지)
        if (next != null)
        {
            if (!ReferenceEquals(currPushHandler, next))
            {
                currPushHandler?.StopPushAttempt();
                currPushHandler = next;
            }
            if (Vector3.Distance(transform.position, next.gameObject.transform.position) < 1.3f)
            {
                currPushHandler.StartPushAttempt(dir4); // 고정 4방향
                currCooltime = pushCooltime;

                return (moveDir, Vector3.zero);
            }
        }
        else
        {
            if (currPushHandler != null)
            {
                currPushHandler.StopPushAttempt();
                currPushHandler = null;
            }
        }
        // 이동은 원래대로
        return (moveDir, Vector3.zero);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 playerCenter = transform.position + Vector3.up * .8f;

        Vector3 debugDir = lastDirN; // 4방향 스냅된 값을 넣어서 기즈모에도 반영

        Vector3 halfExtents = new(.2f, .4f, .2f);
        float maxDist = tileSize * 0.8f;
        float front = Mathf.Max(0.9f, frontOffset);

        Vector3 origin = playerCenter + debugDir * front;

        // BoxCast 시작점
        Gizmos.DrawWireCube(origin, halfExtents * 2);

        // BoxCast 방향선
        Gizmos.DrawLine(origin, origin + debugDir * maxDist);

        // BoxCast 끝부분
        Gizmos.DrawWireCube(origin + debugDir * maxDist, halfExtents * 2);
    }
#endif
}