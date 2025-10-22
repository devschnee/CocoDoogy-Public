using System.Collections;
using UnityEngine;

public class PushableObjects : MonoBehaviour
{
    public float moveTime = 0.12f;
    public float tileSize = 1f;
    [Tooltip("이동 막는 물체")]
    public LayerMask blockingMask;
    [Tooltip("땅 판정")]
    public LayerMask groundMask;
    [Tooltip("경사로")]
    public LayerMask slopeMask;

    private bool isMoving = false;
    private bool isHoling = false;
    private bool isFalling = false;
    public float requiredHoldtime = 0.5f;
    private float currHold = 0f;
    private Vector2Int holdDir;

    public bool allowFall = true;
    public bool allowSlope = false;

    void Update()
    {
        if (!isHoling || isMoving) return;
        currHold += Time.deltaTime;
        if(currHold >= requiredHoldtime)
        {
            TryPush(holdDir);
            currHold = 0f;
            isHoling = false;
        }
    }

    public bool TryPush(Vector2Int dir)
    {
        if (isMoving || isFalling) return false;

        Vector3 offset = new Vector3(dir.x, 0f, dir.y) * tileSize;
        // XZ로만 이동할 목표 먼저 계산.
        Vector3 target = transform.position + offset;

        // 경사로
        if (allowSlope && Physics.Raycast(target + Vector3.up * 0.5f, Vector3.down, 1f, slopeMask))
        {
            Vector3 up = target + Vector3.up * tileSize;
            if (!Physics.CheckBox(up + Vector3.up * 0.5f, Vector3.one * 0.4f, Quaternion.identity, blockingMask) &&
                Physics.Raycast(up + Vector3.up * 0.1f, Vector3.down, 1.5f, groundMask))
            {
                StartCoroutine(MoveTo(up));
                return true;
            }
        }

        // 목적지에 뭔가 있으면 못 감
        if (Physics.CheckBox(target + Vector3.up * 0.5f, Vector3.one * 0.4f, Quaternion.identity, blockingMask))
            return false;

        StartCoroutine(MoveAndFall(target));
        return true;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    IEnumerator MoveAndFall(Vector3 target)
    {
        yield return StartCoroutine(MoveTo(target));

        // 낙하 조건 확인 및 처리
        if (allowFall)
        {
            yield return StartCoroutine(CheckFall());
        }
    }

    IEnumerator CheckFall()
    {
        isFalling = true;

        Vector3 currPos = transform.position;

        while(!Physics.Raycast(currPos + Vector3.up * 0.1f, Vector3.down, 1.5f, groundMask))
        {
            Vector3 fallTarget = currPos + Vector3.down * tileSize;

            if(fallTarget.y < -100f) // 무한 추락 방지
            {
                isFalling = false;
                yield break;
            }

            yield return StartCoroutine(MoveTo(fallTarget));
            currPos = transform.position;
        }
        isFalling = false;
    }

    public void StartPushAttempt(Vector2Int dir)
    {
        if(isMoving || isFalling) return;
        if(isHoling && dir != holdDir)
        {
            currHold = 0f;
        }

        holdDir = dir;
        isHoling = true;
    }

    public void StopPushAttempt()
    {
        isHoling = false;
        currHold = 0f;
    }
}
