using UnityEngine;

public class PushableBox : PushableObjects
{
    [Header("Blocking (Box)")]
    [Tooltip("벽/기믹/문/구멍/플랫폼 + Pushable(다른 박스)까지 전부 포함. 통과가능한 바닥은 X")]
    public LayerMask blockingMask;


    protected override void Awake()
    {
        base.Awake();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // 상자 이동은 높이 보정 없이 평면 1칸
    protected override Vector3 GetNextCellCenter(Vector3 currCenter, Vector3 axis)
    {
        return base.GetNextCellCenter(currCenter, axis);
    }

    protected override bool CanEnterCell(Vector3 nextCenter, Vector3 axis)
    {
        // 그 칸에 뭐라도 있으면 못 들어감
        float half = tileSize * 0.45f;
        Vector3 halfExt = new Vector3(half, tileSize * 0.6f, half);

        return !Physics.CheckBox(nextCenter, halfExt, Quaternion.identity,
                                 blockingMask, QueryTriggerInteraction.Ignore);
    }
}
