using UnityEngine;

public class FlowWaterStrategy : IFlowStrategy
{
    // 이동 코루틴은 PushableObjects 내부에서 처리
    // LSH 추가 1201 ImmediatePush bool 파라미터 추가 flow로 물체 움직일 때는 오브젝트 미는 소리 안나게
    public void ExecuteFlow(PushableObjects target, Vector2Int flowDir)
    {
        target.ImmediatePush(flowDir, false);
    }
}