using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IRider
{
    #region Variables
    [Header("Refs")]
    public Joystick joystick;
    public Rigidbody rb;
    public List<IMoveStrategy> moveStrategies;

    [Header("Player Movement Smoothing")]
    [Tooltip("값 낮을수록 민첩")][Range(0.05f, 0.15f)] public float smoothTime = 0.07f;
    private Vector3 currVel = Vector3.zero;
    private Vector3 velRef = Vector3.zero;

    // 캐릭터의 현재 이동 방향을 월드 좌표계에 맞게 변환하기 위해 필요
    private Transform camTr; // NOTE : 비워두면 자동으로 Camera.main을 사용


    [Header("Move")]
    public float moveSpeed = 3.0f;
    public float accel = 25f;
    public float rotateLerp = 10f;
    public LayerMask slopeMask; // 경사로에 올라타기를 허용시키기 위해. PlayerSlope.cs와 역할이 다름.
                                // 살짝 겹치는 내용 있을 수 있어서 전체적으로 리팩터링 하면 좋긴 함.


    private Vector3 lastValidPos;


    //LSH 추가
    public bool isRunning
    {
        get
        {
            // KHJ - 조이스틱의 두 손가락 드래그 상태일 경우도 false 반환
            if (joystick != null && joystick.IsTwoFingerMode)
                return false;
            Vector2 input = new Vector2(joystick.InputDir.x, joystick.InputDir.z);
            return input.sqrMagnitude > 0.01f;
        }
    }
    //

    public bool isMoveLocked = false;
    private float moveLockTimer;
    #endregion


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camTr = Camera.main != null ? Camera.main.transform : null;


        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        rb.interpolation = RigidbodyInterpolation.None;


        // Strategy Pattern: 시작 시 전략 컴포넌트들을 가져옴
        moveStrategies = new List<IMoveStrategy>(GetComponents<IMoveStrategy>());
    }


    void FixedUpdate()
    {
        if (joystick == null) return;
        if (camTr == null) camTr = Camera.main?.transform;

        if(moveLockTimer > 0f)
        {
            moveLockTimer -= Time.fixedDeltaTime;
            if(moveLockTimer <= 0f)
            {
                moveLockTimer = 0f;
                isMoveLocked = false;
            }
        }

        if (isMoveLocked)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            if(moveStrategies != null)
            {
                foreach(var strategy in moveStrategies)
                {
                    strategy.Execute(Vector3.zero, rb, this);
                }
            }
            return;
        }
        // 두 손가락 드래그 중에는 플레이어 이동/회전 차단
        if (joystick.IsTwoFingerMode)
        {
            // 플레이어 입력 차단 (조이스틱에서 InputDir = Vector3.zero 처리되었지만, 만약을 위해 다시 확인)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);


            // Strategy Pattern: 입력이 0일 때도 Push 전략이 StopPushAttempt를 호출하도록 실행.
            if (moveStrategies != null)
            {
                foreach (var strategy in moveStrategies)
                    strategy.Execute(Vector3.zero, rb, this);
            }
            return; // 이후의 모든 이동 로직을 건너뜀
        }
        Vector2 input = new Vector2(joystick.InputDir.x, joystick.InputDir.z);

        if (input.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);


            // Strategy Pattern: 입력이 0일 때도 Push 전략이 StopPushAttempt를 호출하도록 실행.
            if (moveStrategies != null)
            {
                foreach (var strategy in moveStrategies)
                    strategy.Execute(Vector3.zero, rb, this);
                // 입력 없음 상태도 전략이 받아야 함. 그래서 zero로라도 실행.
            }
            // 입력 없을 때 회전 시키는 것 차단. return 없으면 자동으로 0,0,0으로 돌아감.
            return;
        }


        Vector3 fwd = camTr ? camTr.forward : Vector3.forward;
        Vector3 right = camTr ? camTr.right : Vector3.right;
        fwd.y = 0;
        right.y = 0;
        fwd.Normalize();
        right.Normalize();


        Vector3 moveDir = (right * input.x) + (fwd * input.y);
        if (moveDir.sqrMagnitude > 0.1f) moveDir.Normalize();


        Vector3 finalDir = moveDir;
        Vector3 stepOffset = Vector3.zero;


        // NOTE: Strategy Pattern 적용 - 모든 상호작용을 전략에게 위임
        if (moveStrategies != null)
        {
            // 전략은 순서대로 실행되며, 이전 전략이 반환한 finalDir를 다음 전략에 전달
            foreach (var strategy in moveStrategies)
            {
                Vector3 currentDir = finalDir;
                Vector3 currentOffset = stepOffset;


                (Vector3 newDir, Vector3 newOffset) = strategy.Execute(currentDir, rb, this);

                // 경사 보정 전략은 finalDir를 변경 (ProjectOnPlane)
                // 스텝 전략은 stepOffset을 변경
                finalDir = newDir;
                stepOffset += newOffset;
            }
        }


        // 위치 이동
        Vector3 targetVel = finalDir * moveSpeed;

        currVel = Vector3.SmoothDamp(
            currVel,
            targetVel,
            ref velRef,
            smoothTime
        );

        Vector3 nextPos = rb.position + currVel * Time.fixedDeltaTime + stepOffset;
       
        rb.MovePosition(nextPos);


        // 회전 처리
        float t = Mathf.Clamp01(rotateLerp * Time.fixedDeltaTime);

        // 회전 시 떨림 보정 Deadzone + 정규화
        Vector3 lookInput = new Vector3(joystick.InputDir.x, 0, joystick.InputDir.z);
        if (lookInput.sqrMagnitude > 0.0005f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookInput.normalized, Vector3.up);
            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetRot, t);
            rb.MoveRotation(smoothRot);
        }
    }

    // 인터랙션 이후 투명 콜라이더가 켜지기 전 넘어가버리는 특수 상황처리를 위한 움직임 잠금 메서드
    public void LockMove(float duration)
    {
        if (duration <= 0f) return;
        isMoveLocked = true;
        moveLockTimer = Mathf.Max(moveLockTimer, duration);
    }

    public Vector2Int To4Dir(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            return dir.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            return dir.z > 0 ? Vector2Int.up : Vector2Int.down;
    }


    public void OnStartRiding()
    {
        joystick.gameObject.SetActive(false);
    }


    public void OnStopRiding()
    {
        joystick.gameObject.SetActive(true);
        transform.SetParent(null);
    }
}