using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 모바일 가상 조이스틱 입력을 처리하는 UI 컴포넌트.
/// 한 손가락 입력은 플레이어 이동용 방향 벡터를 생성하고,
/// 두 손가락 입력은 카메라 둘로보기(Look Around) 모드로 전환함.
/// 입력 안정화를 위해 방향 스냅과 시각적 하이라이트를 포함함.
/// </summary>
public class Joystick : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image handle;
    public float moveRange = 75f; // 핸들 움직임 최대 반경(pixel)
    [SerializeField] private Image[] fourSlices; // 조이스틱 4방향 하이라이터

    private static float sharedSnapAngleThreshold = 13;
    private static bool sharedEnhanceFourDir = true;
    private float snapAngleThreshold = 13f;
    private bool enhanceFourDir;

    // 조이스틱 기본 위치
    private Vector3 initPos;
    public Vector3 InputDir { get; private set; }


    // 조이스틱 터치 위치 계산에 사용
    private RectTransform rectTransform;


    // CamControl 참조 및 상태 변수
    private CamControl camCon;
    public bool IsTwoFingerMode { get; private set; } = false;


    private Vector2 lastTwoFingerPos;

    // 패널 켜졌을 떄 조이스틱을 잠그기 위한 변수
    public bool IsLocked { get; set; } = false;

   
    IEnumerator Start()
    {
        snapAngleThreshold = sharedSnapAngleThreshold >= 0 ? sharedSnapAngleThreshold : snapAngleThreshold;
        enhanceFourDir = sharedEnhanceFourDir;
        

        rectTransform = GetComponent<RectTransform>();
        initPos = rectTransform.anchoredPosition;
        InputDir = Vector3.zero;


        camCon = Camera.main?.GetComponent<CamControl>();
        
        
        yield return null;
        onUiSetup?.Invoke(snapAngleThreshold, enhanceFourDir);

        ResetJoystick();
    }


    void Update()
    {
        if (IsLocked)
        {
            ResetJoystick();
            return;
        }

        if (Touchscreen.current == null) return;

        // Touch 모드 분기
        int touchCnt = 0;
        var touches = Touchscreen.current.touches;


        // 터치 몇 개 들어왔는지 카운팅
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].press.isPressed)
                touchCnt++;
        }

        // 모드 종료 확인 (터치 개수가 2개 미만이고, 모드가 켜져 있다면 종료하고 카메라 복귀)
        // 모드 종료 조건 (touchCount < 2)을 먼저 확인하고,
        // 모드가 활성화 상태(IsLookAroundMode == true)인지 확인하여 Reset을 실행
        // 두 손가락 터치 로직
        // touchCnt가 0 또는 1이 되었다면 카메라를 즉시 복귀 시켜야 하기 때문에 0, 1보다 반드시 먼저 실행돼야 함.
        if (touchCnt < 2 && IsTwoFingerMode == true)
        {
            ResetTwoFingerMode(); // IsTwoRingerMode = false <- 카메라 플레이어 추적 시작
        }


        // 터치 입력이 아무 것도 안 들어왔을 때 조이스틱 리셋
        if (touchCnt == 0)
        {
            ResetJoystick();
            return;
        }

        // 두 손가락 터치 모드 시작 및 드래그
        if (touchCnt == 2)
        {
            // 조이스틱의 UI를 복귀시킴
            handle.rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.anchoredPosition = initPos;

            // 두 번째 터치가 시작되거나 이미 진행 중일 때
            var touch1 = Touchscreen.current.touches[0];
            var touch2 = Touchscreen.current.touches[1];

            if (touch1.press.isPressed && touch2.press.isPressed)
            {
                // 모드 시작 : 이미 모드가 종료되었거나 새로 시작하는 경우
                // 두 손가락 터치 모드 시작
                if (IsTwoFingerMode == false)
                {
                    IsTwoFingerMode = true;
                    // 두 손가락의 평균 위치를 초기 위치로 설정
                    lastTwoFingerPos = (touch1.position.ReadValue() + touch2.position.ReadValue()) / 2f;

                    // 카메라 플레이어 추적 중단
                    camCon?.SetFollowingPlayer(false);
                }
                // 두 손가락 드래그 처리
                Vector2 currTwoFingerPos = (touch1.position.ReadValue() + touch2.position.ReadValue()) / 2f;
                Vector2 dragDelta = currTwoFingerPos - lastTwoFingerPos;

                camCon?.LookAroundUpdate(dragDelta);
                lastTwoFingerPos = currTwoFingerPos;

                // Input의 입력을 0으로(드래그 할 때 플레이어가 움직이면 안 됨)
                InputDir = Vector3.zero;
                return; // 조이스틱 일반 로직 스킵
            }
        }

        // 일반 조이스틱 터치 로직(한 손가락 터치)
        if (touchCnt == 1)
        {
            var touch = Touchscreen.current.touches[0];
            //터치 위치로 레이를 쏴서, UI요소가 있으면 리턴시키기 근데이제 조이스틱 자체는 제외해줘야 함
            if (touch.press.wasReleasedThisFrame)//터치가 종료/취소될 때
            {
                ResetJoystick();
            }
            if (touch.press.isPressed) //를 제외하고 터치가 이동중/정지중일 때
            {
                //터치 시작 위치에 UI 요소가 있을 경우 조이스틱 입력으로 처리하지 않고 무시
                List<RaycastResult> results = new();

                PointerEventData data = new(EventSystem.current);
                data.position = touch.startPosition.ReadValue();
                EventSystem.current.RaycastAll(data, results);
                foreach (var r in results)
                {
                    if (r.gameObject)
                    {
                        return;
                    }
                }
                // 조이스틱 기준 위치가 터치 시작 지점에서 멀 경우
                // 새로운 터치 위치로 조이스틱 기준점 재설정
                transform.position = Vector2.Distance(transform.position, touch.startPosition.ReadValue()) < 150 ? transform.position : touch.startPosition.ReadValue();

                //가상 조이스틱 핸들을 터치 위치로 옮기되, 범위를 넘어가지 않게 하면 됨.
                Drag((Vector3)touch.position.ReadValue() - transform.position);
            }
        }
    }

    // 터치가 드래그 될 때
    public void Drag(PointerEventData eventData)
    {
        RectTransform bgRect = bg.rectTransform;
        
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRect, eventData.position, eventData.pressEventCamera, out Vector2 pos)) return;


        // 핸들 이동반경 제한
        Vector2 clamped = Vector2.ClampMagnitude(pos, moveRange);
        handle.rectTransform.anchoredPosition = clamped;

        Vector2 inputNormal = clamped.sqrMagnitude > 0.0001f ? (clamped / moveRange) : Vector2.zero;

        //핸들 각도 스냅(8방향 기준으로)
        Vector2 snapped = SnapDirection(inputNormal, enhanceFourDir);
        
        InputDir = new Vector3(snapped.x, 0, snapped.y);
    }

    private readonly static Vector2[] eightDir = new Vector2[8]
    {
                    Vector2.up,
                    (Vector2.up + Vector2.right).normalized,
                    Vector2.right,
                    (Vector2.down + Vector2.right).normalized,
                    Vector2.down,
                    (Vector2.down + Vector2.left).normalized,
                    Vector2.left,
                    (Vector2.up + Vector2.left).normalized
    };

    // 입력 벡터를 8방향 기준으로 스냅 처리.
    // enhancedFourDir가 활성화된 경우 상화좌우 방향의 허용 각도를 확장하여 4방향 이동이 더 쉽게 선택되도록 보정.
    private Vector2 SnapDirection(Vector2 inputVector, bool enhanceFourdir)
    {
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 snapVector = eightDir[i];
                float currentThreshold = snapAngleThreshold;
                if (enhanceFourdir) { if (i % 2 == 0) { currentThreshold *= 2; } }
                if (Vector2.Angle(inputVector, snapVector) < currentThreshold)
                {
                    inputVector = snapVector * inputVector.magnitude;
                    break;
                }
            }

            
        }

        return inputVector;
    }

    // 터치 위치 기반으로 조이스틱 입력을 처리하기 위한 Drag 오버로드
    public void Drag(Vector2 pos)
    {
        // 핸들 이동반경 제한
        Vector2 clamped = Vector2.ClampMagnitude(pos, moveRange);
        handle.rectTransform.anchoredPosition = clamped;


        Vector2 inputNormal = clamped.sqrMagnitude > 0.0001f ? (clamped / moveRange) : Vector2.zero;

        //핸들 각도 스냅(8방향 기준으로)
        inputNormal = SnapDirection(inputNormal, enhanceFourDir);

        FourDirHighlightUI(InputDir);
        InputDir = new Vector3(inputNormal.x, 0, inputNormal.y);
    }


    // 두 손가락 터치 모드 종료 처리
    // 카메라 LookAround 모드 해제
    // 다시 플레이어 추적 모드로 복귀
    public void ResetTwoFingerMode()
    {
        IsTwoFingerMode = false;
        InputDir = Vector3.zero; // 조이스틱 재활성화 준비
        lastTwoFingerPos = Vector2.zero;
        camCon?.SetFollowingPlayer(true); // 즉시 플레이어 따라가기
    }

    public void ResetJoystick()
    {
        InputDir = Vector3.zero;
        handle.rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchoredPosition = initPos;
        for (int i = 0; i < fourSlices.Length; i++) fourSlices[i].enabled = false;
    }

    // 현재 입력 방향에 따라 상/하/좌/우 하이라이트 UI 활성화
    private void FourDirHighlightUI(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
        {
            for (int i = 0; i < fourSlices.Length; i++) fourSlices[i].enabled = false;
            return;
        }

        int idx;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) idx = dir.x > 0 ? 1 : 3; // Right 1 Left 3
        else idx = dir.z > 0 ? 0 : 2; // Up 0 Down 2

        for (int i = 0; i < fourSlices.Length; i++) fourSlices[i].enabled = (i == idx);
    }

    public void ApplyOptions(float snapAngleThreshold, bool enhanceFourDir)
    {
        this.snapAngleThreshold = snapAngleThreshold;
        this.enhanceFourDir = enhanceFourDir;

        sharedSnapAngleThreshold = snapAngleThreshold;
        sharedEnhanceFourDir = enhanceFourDir;
    }

    public Action<float, bool> onUiSetup;
}