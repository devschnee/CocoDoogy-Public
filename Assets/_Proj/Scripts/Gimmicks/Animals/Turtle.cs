using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 빙판형 이동을 수행하는 거북이 기믹 오브젝트.
/// 4방향 이동, 상단 탑승 객체(Ridable) 탐색 및 위치 동기화.
/// IRider 콜백을 통해 탑승/하차 시점 통지 
/// 가장자리 충돌 안정을 위해 IEdgeColliderHandler를 통한 지형 경계면(Edge) 보정 로직 사용
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class Turtle : MonoBehaviour, IDashDirection, IPlayerFinder
{
    [Header("Movement Settgins")]
    public float tileSize = 1f;
    [Tooltip("이동 속도 (tile/s)")]
    public float moveSpeed = 5f;
    [Tooltip("거북이 충돌 레이어")]
    public LayerMask blockLayer;
    [Tooltip("터틀 위에 올라탈 수 있는 오브젝트의 레이어 이름")]
    public LayerMask ridableLayer;

    [Header("Player Detection")]
    public float detectRadius = 3f; // 플레이어 감지 범위
    public LayerMask playerLayer; // 플레이어 레이어 마스크
    private Transform playerTrans; // 감지된 플레이어의 Transform


    public float playerLockSec;

    private bool isMoving = false;

    public bool IsMoving => isMoving;
    private Vector3 targetPos;
    private Rigidbody rb; // 물리 처리용 (물살 영향 방지)

    [Header("Popup Buttons")]
    [SerializeField] private GameObject btnGroup;
    [SerializeField] private Button up;
    [SerializeField] private Button down;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] Image typeIcon;

    public bool CanInteract => !isMoving; // 이동 중이 아닐 때만 상호작용 가능

    Transform IPlayerFinder.Player { get => playerTrans; set => playerTrans = value; }

    void Awake()
    {
        // ETCEvent.Invoke... => 소리
        up.onClick.AddListener(() => { ETCEvent.InvokeCocoInteractSoundInGame(); GetDirection(new Vector2Int(0, 1)); btnGroup.SetActive(false); });
        down.onClick.AddListener(() => { ETCEvent.InvokeCocoInteractSoundInGame(); GetDirection(new Vector2Int(0, -1)); btnGroup.SetActive(false); });
        left.onClick.AddListener(() => { ETCEvent.InvokeCocoInteractSoundInGame(); GetDirection(new Vector2Int(-1, 0)); btnGroup.SetActive(false); });
        right.onClick.AddListener(() => { ETCEvent.InvokeCocoInteractSoundInGame(); GetDirection(new Vector2Int(1, 0)); btnGroup.SetActive(false); });

        btnGroup.SetActive(false);

        // 플레이어 transform 찾기
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTrans = playerGO.transform;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        if (!playerTrans || !btnGroup) return;
        // 대화가 생성되면 버튼을 숨김
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            if (btnGroup.activeSelf)
                btnGroup.SetActive(false);
            return;
        }
        float dist = Vector3.Distance(transform.position + Vector3.up * 0.5f, playerTrans.position);
        bool inRange = dist <= detectRadius;

        if (btnGroup.activeSelf != inRange)
        {
            btnGroup.SetActive(inRange);
            if (inRange)
            {
                // NOTE : 추후 각도 변경 가능성 있음.
                btnGroup.transform.rotation = Quaternion.Euler(75f, 0, 0);
            }
        }
    }

    public void GetDirection(Vector2Int dir)
    {
        if (isMoving) return;

        isMoving = true;
        btnGroup.SetActive(false);

        if (playerTrans != null)
        {
            var pm = playerTrans.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                // 인터랙션 눌리면 잠깐동안 플레이어 움직임 잠금
                pm.LockMove(playerLockSec);
            }
        }
        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);

        // 빙판 로직 적용 : 멈출 때까지 방향으로 이동할 목표 위치 계산
        targetPos = CalculateSlideTarget(moveDir);

        // 이동 시작 시, 위에 얹힌 모든 오브젝트도 같이 이동 시작
        StartCoroutine(MoveSlideCoroutine(moveDir, transform.position, targetPos));
    }

    // 빙판 로직 계산
    private Vector3 CalculateSlideTarget(Vector3 dir)
    {
        Vector3 currTile = transform.position;
        Vector3 nextTile = currTile + dir * tileSize;

        // 충돌 체크용 박스 크기
        Vector3 boxHalfExt = Vector3.one * (tileSize * 0.45f) + Vector3.up * 0.25f;

        // Block Layer를 만날 때까지 계속 이동
        while (true)
        {
            Collider[] hits = Physics.OverlapBox(
                nextTile,
                boxHalfExt,
                Quaternion.identity,
                blockLayer,
                QueryTriggerInteraction.Ignore
            );

            // 충돌 발생하면 break
            if (hits.Length > 0)
            {
#if UNITY_EDITOR
                foreach (var hit in hits)
                {
                    string layerName = LayerMask.LayerToName(hit.gameObject.layer);
                }
#endif
                break;
            }
            // 이동 조건이 충족되었으므로 다음 타일로 이동
            currTile = nextTile;
            nextTile += dir * tileSize;
        }
        return currTile; // 블록 타일 바로 앞 타일 위치 반환
    }

    // 탑승 물체 이동
    IEnumerator MoveSlideCoroutine(Vector3 dir, Vector3 startPos, Vector3 endPos)
    {
        //아래 변수는 이동을 시작할 때 거북이 사방의 땅 블록 리스트임.
        List<IEdgeColliderHandler> startCache = GetComponent<IEdgeColliderHandler>().DetectGrounds();

        float dist = Vector3.Distance(startPos, endPos);
        if (dist < tileSize * 0.5f)
        {
            isMoving = false;
            yield break;
        }

        float duration = dist / moveSpeed;
        float elapsed = 0f;

        Vector3 offset = endPos - startPos;

        // 탑승 가능한 오브젝트 감지
        Vector3 overlapOrigin = transform.position - Vector3.up * (tileSize / 2f);
        Vector3 halfExtents = new Vector3(tileSize * 0.45f, tileSize * 10f, tileSize * 0.45f);

        Collider[] ridables = Physics.OverlapBox(
            overlapOrigin,
            halfExtents,
            Quaternion.identity,
            ridableLayer,
            QueryTriggerInteraction.Ignore
        );

        List<Transform> ridableTrans = new List<Transform>();
        List<Vector3> ridableTargetPos = new List<Vector3>();
        List<Rigidbody> ridableRbs = new List<Rigidbody>();

        //거북이가 직접 플레이어무브먼트의 enabled상태 조작 금지(플레이어 단에서 조이스틱 해제만 함.)
        List<PushableObjects> pushableObjs = new List<PushableObjects>();

        foreach (var col in ridables)
        {
            if (col.transform == transform) continue;

            Rigidbody riderRb = col.attachedRigidbody;
            PlayerMovement pm = col.GetComponent<PlayerMovement>();
            PushableObjects po = col.GetComponent<PushableObjects>();

            // 탑승 가능한 객체인지 확인
            if (pm == null && po == null) continue;

            // 공통 데이터 추가
            ridableTargetPos.Add(col.transform.position + offset);
            ridableTrans.Add(col.transform);

            // Rigidbody 쓰는 객체 처리
            if (riderRb != null)
            {
                riderRb.isKinematic = true;
                ridableRbs.Add(riderRb); // Rigidbody가 있는 객체만 리스트에 추가
            }

            // PushableObjects 처리
            if (po != null)
            {
                po.enabled = false;
                pushableObjs.Add(po);
            }

            // 탑승 감지 시 IRider 콜백을 즉시 호출하여
            col.GetComponent<IRider>()?.OnStartRiding();
            // 탑승 객체가 스스로 상태(부모, 위치 기준)를 초기화하도록 처리
            col.transform.SetParent(transform);
        }


        // 터틀과 탑승 물체 이동 동기화
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, t);

            Vector3 hitExt = new Vector3(tileSize * 0.45f, 0.1f, tileSize * 0.45f);

            // blockingLayer 검사
            Collider[] midHits = Physics.OverlapBox(
                nextPos,
                hitExt,
                Quaternion.identity,
                blockLayer,
                QueryTriggerInteraction.Ignore
            );

            // 앞 막힘 판단
            bool blocked = false;

            foreach (var h in midHits)
            {
                // 머리 위에 실려 있는 애들은 blockLayer라도 무시
                if (ridableTrans.Contains(h.transform))
                    continue;

                // 그 외 blockLayer는 진짜로 막힘
                blocked = true;
                break;
            }

            if (blocked)
            {
                // 타일 스냅
                Vector3 snapped = new Vector3(
                    Mathf.Round(transform.position.x / tileSize) * tileSize,
                    transform.position.y,
                    Mathf.Round(transform.position.z / tileSize) * tileSize
                );

                transform.position = snapped;
                endPos = snapped;
                break;
            }

            // 충돌 없으면 원래대로 이동
            transform.position = nextPos;


            // 터틀 이동 중
            //transform.position = Vector3.Lerp(startPos, endPos, t);
            if (dir.sqrMagnitude > 0.001f)
            {
                // 터틀 이동 중 탑승 객체의 위치/회전을 동기화
                // IEdgeColliderHandler를 구현한 객체는
                // 자체 보정 로직을 통해 가장자리 충돌 처리를 수행하도록 위임
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = targetRot;
                if (ridableTrans != null && ridableTrans.Count > 0)
                {
                    foreach (var trans in ridableTrans)
                    {
                        trans.SetPositionAndRotation(
                            trans.GetComponent<PlayerMovement>() == null ?
                            transform.position + (Vector3Int.up * ridableTrans.IndexOf(trans))
                            : transform.position + (Vector3.up * (ridableTrans.IndexOf(trans) - .5f)),

                            transform.rotation);
                    }
                }
                // 0.5초 정도면 이미 첫 위치를 벗어났을 것이라 가정
                // 이동 초기 구간에서 Edge 상태 갱신하여 
                // 위치 변화로 인한 투명 벽/경계 상태 붕괴 방지
                if (elapsed < .5f)
                {
                    GetComponent<IEdgeColliderHandler>().DetectAndApplyFourEdge();
                    startCache.ForEach((x) => x.DetectAndApplyFourEdge());
                }
            }
            yield return null;
        }

        // 이동 완료 및 상태 정리
        transform.position = endPos;

        transform.position = endPos;

        // IRider를 갖고 있는 박스가 적층됐을 때(부모가 Turtle이 아님) 처리를 위해 변경
        // Rider 해제 처리 (1층만 하차)
        foreach (var trans in ridableTrans)
        {
            if (trans == null) continue;
            var rider = trans.GetComponent<IRider>();
            if (rider == null) continue;

            // 부모가 터틀 자신일 때만 하차 처리
            if (trans.parent == transform)
            {
                trans.GetComponent<IRider>()?.OnStopRiding();
                trans.SetParent(null);
            }
            // 이미 다른 Pushable(즉, 2층 이상)의 자식이면 그대로 둠
        }
        yield return null;

        // Rigidbody 가진 객체만 복원
        foreach (var rb in ridableRbs)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        // PushableObjects 재활성화 
        foreach (var po in pushableObjs)
        {
            if (po != null)
            {
                po.enabled = true;

            }
        }
        isMoving = false;


        yield return null;

        List<IEdgeColliderHandler> endCache = GetComponent<IEdgeColliderHandler>().DetectGrounds();

        GetComponent<IEdgeColliderHandler>().DetectAndApplyFourEdge();
        foreach (var h in startCache) h.DetectAndApplyFourEdge();
        foreach (var h in endCache) h.DetectAndApplyFourEdge();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // 플레이어 감지 범위
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, detectRadius);

        // Ridable 오브젝트 감지 범위
        Gizmos.color = Color.cyan * 0.5f;
        Vector3 overlapOrigin = transform.position + Vector3.up * (tileSize / 2f); // Centre
        Vector3 halfExtents = new Vector3(tileSize * 0.45f, tileSize * 0.75f, tileSize * 0.45f);
        Vector3 size = halfExtents * 2f;
        Gizmos.DrawWireCube(overlapOrigin, size);

        // 최종 목표 위치
        if (Application.isPlaying && isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPos, 0.2f);
        }
    }
#endif
}