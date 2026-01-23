using System.Collections;
using UnityEngine;

/// <summary>
/// 물 타일 위에 위치한 PushableObjects를 일정 간격으로 흐름 방향으로 밀어내는 환경 기믹.
/// 물 타일의 방향을 기준으로 흐름 벡터를 계산하며, 실제 이동 처리 로직은 IFlowStrategy에 위임.
/// 낙하 중이거나 이동 중인 오브젝트는 제외하여 퍼즐 규칙의 안정성을 유지.
/// </summary>

namespace Water
{
    public class Flow : MonoBehaviour
    {
        private Material waterMat;
        
        public float flowInterval; // 오브젝트 밀어내는 간격

        private IFlowStrategy flowStrategy = new FlowWaterStrategy();

        private Vector3 flowDir;

        [Tooltip("밀려날 수 있는 오브젝트 레이어")]
        public LayerMask pushableMask;

        private Coroutine flowCoroutine;

        void Awake()
        {
            // Flow 기믹은 Water 레이어로 설정되어야 함 (설정 오류 방지용)
            if (gameObject.layer != LayerMask.NameToLayer("Water")) {}
            
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                waterMat = renderer.material;
            }
        }

        void Start()
        {
            SetFlowDir();

            if (flowCoroutine != null) StopCoroutine(flowCoroutine);
            flowCoroutine = StartCoroutine(FlowObjsCoroutine());
        }

        public void SetFlowDir()
        {
            // 오브젝트 forward 방향을 기준으로 흐름 방향 게산(XZ 평면)
            flowDir = transform.forward;
            flowDir.y = 0f;
            flowDir.Normalize();
            
            if (waterMat != null)
            {
                waterMat.SetVector("_FlowDir", new(-10, 0, 20, 0));
            }
        }

        IEnumerator FlowObjsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(flowInterval);

                Vector3 centre = transform.position;

                // 물 타일 중앙 기준으로 밀려날 수 있는 PushableObjects 감지
                float checkSize = 0.45f;

                // 물 타일 높이의 중앙보다 살짝 위에서 검사
                // 검사 영역은 물 타일 높이를 포함. Y좌표 검증을 통해 걸러내기
                Collider[] hits = Physics.OverlapBox(centre + Vector3.up * 0.5f, Vector3.one * checkSize, Quaternion.identity, pushableMask);

                // 물 타일의 y 좌표를 기준 높이로 설정
                float flowY = centre.y;
                
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<PushableObjects>(out var pushable))
                    {
                        // 가장 아래층 물체의 y좌표가 물 타일의 y좌표와 일치하는지 확인
                        if (Mathf.Abs(pushable.transform.position.y - flowY) > 0.01f) continue;

                        // 물체가 낙하 중이거나 이미 이동 중이라면 제외
                        if (pushable.IsFalling || pushable.IsMoving) continue;

                        // 실제 이동 및 탑승/적층 규칙은 PushableObjects.cs에 위임
                        // 흐름 방향만 2D(Vector2Int)로 전달
                        Vector2Int flowDir2D = new Vector2Int(Mathf.RoundToInt(flowDir.x), Mathf.RoundToInt(flowDir.z));

                        flowStrategy.ExecuteFlow(pushable, flowDir2D);
                    }
                }
            }
        }
    }
}