using TMPro;
using UnityEngine;

/// <summary>
/// 스테이지 선택 화면에서 현재 선택된 스테이지 정보를 UI에 표시하는 컴포넌트.
/// Firebase에 저장된 선택 정보를 기반으로 스테이지 이름을 초기화.
/// </summary>

public class StageTitleInfo : MonoBehaviour
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
        stageNameTxt.text = "< " + data.stage_name + " >";
    }
}
