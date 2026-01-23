using TMPro;
using UnityEngine;

/// <summary>
/// 인게임 HUD에서 현재 스테이지 이름을 표시하는 UI 컴포넌트.
/// 스테이지 진입 시 DataManager를 통해 스테이지 정보를 동기화.
/// </summary>

public class StageTitleIngame : MonoBehaviour
{
    public TextMeshProUGUI stageNameTxt;
    private string currentStageId;

    void Awake()
    {
        currentStageId = FirebaseManager.Instance.selectStageID;
    }

    void Start()
    {
        StageUIManager.Instance.stageIdInformation.stageIdInfo = currentStageId;
        var data = DataManager.Instance.Stage.GetData(currentStageId);
        StageUIManager.Instance.stageName.text = data.stage_name;
        stageNameTxt.text = data.stage_name;
    }
}