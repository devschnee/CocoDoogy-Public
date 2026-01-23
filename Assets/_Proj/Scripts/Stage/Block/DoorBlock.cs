using System.Collections;
using UnityEngine;

/// <summary>
/// 신호 기반으로 개폐되는 문 기믹 오브젝트.
/// ISignalReceiver를 구현하여 Switch, Tower, Turret 등
/// 다양한 신호 발신 기믹으로부터 신호를 수신한다.
/// 연결된 기믹 타입에 따라 초기 상태와 개폐 동작이 달라지며,
/// ShockDetectionTower에 의해 영구 개방되는 상태도 지원한다.
/// </summary>
public class DoorBlock : Block, ISignalReceiver
{
    public bool IsOn { get; set; }

    private bool isPermanentlyOpen = false; // 충격파 감지탑에서 한 번 감지되면 영구적으로 문을 열어야 하므로
    [Tooltip("문이 여닫히는 속도")]
    public float openSpeed = 2f;
    Transform left;
    Transform right;

    private enum GimmickType { Switch, Tower, Turret }
    private GimmickType connectedType;
    private bool initialized;

    private Vector3 leftClosedPos, leftOpenPos;
    private Vector3 rightClosedPos, rightOpenPos;
    private Coroutine doorRoutine;

    private BoxCollider boxCol;

    public void ReceiveSignal()
    {
        if (!initialized) return;

        if (isPermanentlyOpen) // Tower가 수정되면서 더이상 호출되지 않을 것임
        {
            // Tower는 한 번 열면 닫지 않음.
            return;
        }

        AudioEvents.Raise(SFXKey.InGameObject,3, pooled: true, pos: gameObject.transform.position);
        // 기믹 타입에 따라 개폐 조건을 달리 해줌.
        if (connectedType == GimmickType.Turret)
        {
            // 터렛은 감지 -> 닫힘 / 감지 해제 -> 열림
            IsOn = !IsOn;
            ToggleDoor(IsOn);
        }
        else
        {
            IsOn = !IsOn;
            ToggleDoor(IsOn);
        }
    }

    void Awake()
    {
        if (!left)
            left = transform.GetChild(0).GetChild(1);
        if (!right)
            right = transform.GetChild(0).GetChild(2);

        // 기준 위치 기록
        leftClosedPos = left.localPosition;
        rightClosedPos = right.localPosition;

        // 열릴 때 이동할 방향 (로컬 기준)
        float offset = 0.95f;
        leftOpenPos = leftClosedPos + Vector3.right * offset;
        rightOpenPos = rightClosedPos + Vector3.left * offset;

        boxCol = GetComponent<BoxCollider>();
        boxCol.enabled = true;
    }

    void Start()
    {
        StartCoroutine(InitAfterFrame());
    }

    IEnumerator InitAfterFrame()
    {
        yield return null; // 한 프레임 대기 (기믹 초기화 완료 대기)
        DetectConnectedGimmick();
        ApplyInitialState();
        initialized = true;
    }

    // 근처에 있는 연결된 기믹을 찾아서 세팅 해줌.
    // (스테이지 별로 문을 열게 해주는 기믹이 다르고 동작 방식과 기본 세팅이 다르기 때문에)
    void DetectConnectedGimmick()
    {
        float searchRadius = 60f;
        Collider[] cols = Physics.OverlapSphere(transform.position, searchRadius, ~0);

        foreach (var c in cols)
        {
            var sender = c.GetComponentInParent<ISignalSender>();

            if (sender == null) continue;
            //if (sender.Receiver != this) continue;
            if ((DoorBlock)sender.Receiver != this) continue;

            if (sender is SwitchBlock)
                connectedType = GimmickType.Switch;
            else if (sender is ShockDetectionTower || sender is TowerBlock)
                connectedType = GimmickType.Tower;
            else if (sender is Turret || sender is TurretBlock)
                connectedType = GimmickType.Turret;
            return;
        }

        // 연결 실패 시 기본 Switch로 취급
        connectedType = GimmickType.Switch;
    }

    // 연결된 기믹 타입에 따른 초기 상태 세팅
    void ApplyInitialState()
    {
        // Switch, Tower: 기본 닫힘 / Turret: 기본 열림
        bool open = (connectedType == GimmickType.Turret);
        IsOn = open;
        ToggleDoor(open);
    }

    void ToggleDoor(bool open)
    {
        if (doorRoutine != null)
            StopCoroutine(doorRoutine);
        doorRoutine = StartCoroutine(AnimateDoor(open));
        boxCol.enabled = !open;
    }

    IEnumerator AnimateDoor(bool open)
    {
        float t = 0f;
        Vector3 lStart = left.localPosition;
        Vector3 rStart = right.localPosition;
        Vector3 lTarget = open ? leftOpenPos : leftClosedPos;
        Vector3 rTarget = open ? rightOpenPos : rightClosedPos;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            float smooth = Mathf.SmoothStep(0, 1, t);

            left.localPosition = Vector3.Lerp(lStart, lTarget, smooth);
            right.localPosition = Vector3.Lerp(rStart, rTarget, smooth);
            yield return null;
        }

        left.localPosition = lTarget;
        right.localPosition = rTarget;
    }

    // ShockDetectionTower를 위한 영구적으로 문 열기 메서드
    public void OpenPermanently()
    {
        if (isPermanentlyOpen) return;

        isPermanentlyOpen = true;

        if (!IsOn)
        {
            IsOn = true;
            ToggleDoor(true);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float searchRadius = 60f; // DetectConnectedGimmick에서 사용되는 searchRadius와 동일하게 설정
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
#endif
}