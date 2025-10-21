using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushMinSpeed = 0.1f;    // 너무 느리면 푸시 무시
    public float pushCooldown = 0.08f;   // 과연속 방지

    Rigidbody rb;
    float lastPushTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision col)
    {
        // 플레이어 이동 속도 체크
        Vector3 v = rb.linearVelocity; v.y = 0f;
        if (v.sqrMagnitude < pushMinSpeed * pushMinSpeed) return;

        // IPushable 찾기(부모/자식 어디 있어도 OK)
        if (!col.collider.TryGetComponent<IPushable>(out var pushable))
        {
            pushable = col.collider.GetComponentInParent<IPushable>();
            if (pushable == null) pushable = col.collider.GetComponentInChildren<IPushable>();
            if (pushable == null) return;
        }

        if (pushable.IsMoving) return;
        if (Time.time - lastPushTime < pushCooldown) return;

        // 축 정렬된 밀 방향
        Vector3 dir = v.normalized;
        Vector3 axis = (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
            ? new Vector3(Mathf.Sign(dir.x), 0f, 0f)
            : new Vector3(0f, 0f, Mathf.Sign(dir.z));

        // 한 칸 밀기 요청
        if (pushable.RequestPush(axis))
        {
            lastPushTime = Time.time;
        }
    }
}
