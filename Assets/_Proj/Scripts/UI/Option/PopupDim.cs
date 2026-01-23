using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 팝업 활성화 시 옵션 패널 입력을 차단하는 Dim 컨트롤러.
/// 팝업 영역을 제외한 UI Selectable을 비활성화하여 입력 충돌을 방지.
/// </summary>

public class PopupDim : MonoBehaviour
{
    [SerializeField] private GameObject optionPanelGroup; // OptionPanel 전체
    [SerializeField] private GameObject popupsGroup; // Popups 루트
    [SerializeField] private Image dimBackground; // Dim

    private List<Selectable> disabledSelectables = new List<Selectable>();

    public static PopupDim Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        // Dim은 반드시 RaycastTarget ON
        if (dimBackground)
            dimBackground.raycastTarget = true;
    }

    public void Block()
    {
        // Dim 켜기
        dimBackground.color = new Color(0, 0, 0, 0.45f);
        gameObject.SetActive(true);

        DisableOnlyOptionPanelUIs();
    }

    public void Unblock()
    {
        // Dim 끄기
        dimBackground.color = new Color(0, 0, 0, 0f);
        gameObject.SetActive(false);

        RestoreSelectables();
    }

    private void DisableOnlyOptionPanelUIs()
    {
        disabledSelectables.Clear();

        // OptionPanel 전체 Selectable 검색
        var all = optionPanelGroup.GetComponentsInChildren<Selectable>(true);

        foreach (var s in all)
        {
            if (s.transform.IsChildOf(popupsGroup.transform))
                continue;

            if (s.interactable)
            {
                s.interactable = false;
                disabledSelectables.Add(s);
            }
        }
    }
    private void RestoreSelectables()
    {
        foreach (var s in disabledSelectables)
            s.interactable = true;

        disabledSelectables.Clear();
    }
}
