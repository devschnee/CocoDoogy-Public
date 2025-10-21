using UnityEngine;

public class PushableOrb : PushableObjects
{
    [Header("Blocking (Orb)")]
    [Tooltip("벽/기믹/문/구멍/플랫폼 + Pushable(다른 박스/구슬 포함) 통과 가능한 바닥은 X")]
    public LayerMask blockingMask;

    [Header("Slope")]
    [Tooltip("경사로 레이어 (경사 Mesh/Collider)")]
    public LayerMask rampMask;
    [Tooltip("다음 칸 위를 샘플링할 높이 오프셋")]
    public float sampleHeight = 2.0f;

    protected override Vector3 GetNextCellCenter(Vector3 currCenter, Vector3 axis)
    {
        Vector3 baseCenter = base.GetNextCellCenter(currCenter, axis);

        // 경사로 표면 높이로 Y 보정
        Vector3 from = baseCenter + Vector3.up * sampleHeight;
        if (Physics.Raycast(from, Vector3.down, out RaycastHit hit, sampleHeight * 2f, rampMask, QueryTriggerInteraction.Ignore))
        {
            baseCenter.y = hit.point.y; // 경사 표면 높이로 세팅
        }
        return baseCenter;
    }

    protected override bool CanEnterCell(Vector3 nextCenter, Vector3 axis)
    {
        // 칸이 막혀있으면 못 감(경사로 표면 자체는 blockingMask에 포함되지 않도록 세팅)
        float half = tileSize * 0.45f;
        Vector3 halfExt = new Vector3(half, tileSize * 0.6f, half);

        return !Physics.CheckBox(nextCenter, halfExt, Quaternion.identity,
                                 blockingMask, QueryTriggerInteraction.Ignore);
    }
}
