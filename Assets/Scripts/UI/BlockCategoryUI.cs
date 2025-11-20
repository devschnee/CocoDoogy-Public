using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BlockCategoryUI : MonoBehaviour
{
    public GameObject blockButtonPrefab; // 버튼 프리팹
    public Transform blockAnimalGimick;
    public Transform blockListParent; // HorizontalLayoutGroup을 가진 오브젝트

    [Header("카테고리별 블록 목록")]
    public List<BlockListData> allCategories; // 블록, 동물, 기믹 등

    //public List<BlockListData> blockData; // 1번 카테고리 (예: 블록)
    //public List<BlockListData> animalData; // 2번 카테고리 (예: 동물)
    //public List<BlockListData> gimickData; // 3번 카테고리 (예: 기믹)

    private List<GameObject> spawnedCategoryButtons = new List<GameObject>();
    private List<GameObject> spawnedButtons = new List<GameObject>();
    private List<BlockData> currentList; // 현재 표시 중인 블록 리스트

    void Start()
    {
        CreateCategoryButtons(); // 시작 시 카테고리 버튼 자동 생성
    }

    void Update()
    {
        // 카테고리 전환
        //if (Input.GetKeyDown(KeyCode.F1))
        //    ShowBlocks(blockData);
        //if (Input.GetKeyDown(KeyCode.F2))
        //    ShowBlocks(animalData);
        //if (Input.GetKeyDown(KeyCode.F3))
        //    ShowBlocks(gimickData);
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    ClearBlocks();
        //    EditorManager.Instance.EditorMode = EditorMode.Idle;
        //}

        // 단축키 블록 선택 (1~9)
        HandleCategoryHotkeys();
        HandleBlockHotkeys();
    }

    public void ShowBlocks(BlockListData category)
    {
        ClearBlocks();
        currentList = new List<BlockData>(category.blocks);

        for (int i = 0; i < currentList.Count; i++)
        {
            var data = currentList[i];
            var go = Instantiate(blockButtonPrefab, blockListParent);
            go.GetComponent<Image>().sprite = data.icon;
            go.GetComponent<Button>().onClick.AddListener(() => EditorManager.Instance.ChooseBlock(data));
            spawnedButtons.Add(go);

            // 숫자 표시 (TextMeshPro)
            var keyText = go.transform.Find("KeyText")?.GetComponent<TextMeshProUGUI>();
            if (keyText != null)
                keyText.text = (i + 1).ToString();
        EditorManager.Instance.EditorMode = EditorMode.Place;
        }
    }

    public void ClearBlocks()
    {
        foreach (var btn in spawnedButtons)
            Destroy(btn);
        spawnedButtons.Clear();
        
    }

    private void CreateCategoryButtons()
    {
        foreach (var category in allCategories)
        {
            var go = Instantiate(blockButtonPrefab, blockAnimalGimick);
            var button = go.GetComponent<Button>();
            var image = go.GetComponent<Image>();
            image.sprite = category.icon;
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = category.categoryName;

            //button.onClick.AddListener(() => ShowBlocks(category));
            spawnedCategoryButtons.Add(go);
        }
    }
    private void HandleCategoryHotkeys()
    {
        if (allCategories == null || allCategories.Count == 0) return;
        for (int i = 0; i < allCategories.Count; i++)
        {
            KeyCode funcKey = KeyCode.F1 + i;
            if (Input.GetKeyDown(funcKey))
            {
                ShowBlocks(allCategories[i]);
            }
        }
        //if (Input.GetKeyDown(KeyCode.F1))
        //    ShowBlocks(allCategories[0]);

        //if (Input.GetKeyDown(KeyCode.F2))
        //    ShowBlocks(allCategories[1]);

        //if (Input.GetKeyDown(KeyCode.F3))
        //    ShowBlocks(allCategories[2]);

        //if (Input.GetKeyDown(KeyCode.F4))
        //    ShowBlocks(allCategories[3]);
        //Todo : 테마 추가(allCategories에 추가 ScirptableObject가 생길시 여기에 코드 추가
        //예시

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearBlocks();
            EditorManager.Instance.EditorMode = EditorMode.Idle;
        }
    }
    private void HandleBlockHotkeys()
    {
        if (currentList == null || currentList.Count == 0) return;

        //에디터매니저의 모드가 배치모드가 아닐 경우 동작 안하도록 처리
        if (EditorManager.Instance.EditorMode != EditorMode.Place) return;
        for (int i = 0; i < currentList.Count && i < 9; i++)
        {
            // 숫자키 1~9 확인
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
            {
                EditorManager.Instance.ChooseBlock(currentList[i]);
            }
        }
    }
}