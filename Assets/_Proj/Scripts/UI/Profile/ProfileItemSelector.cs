using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileItemSelector : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Transform slotParent;
    [SerializeField] private ProfileSlot slotPrefab;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text titleText;

    private string currentCategory;
    private int selectedItemId = -1;

    private ProfileFavoriteIcon currentTargetIcon;
    private ProfilePanelController parentController;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        if (applyButton != null)
            applyButton.onClick.AddListener(Apply);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

    }

    public void Open(string category, ProfileFavoriteIcon targetIcon, ProfilePanelController parent)
    {
        if (root != null)
            root.SetActive(true);

        currentCategory = category;
        currentTargetIcon = targetIcon;
        parentController = parent;

        if (titleText != null)
            titleText.text = $"{category}";

        BuildList();
    }

    private void BuildList()
    {
        foreach (Transform t in slotParent)
            Destroy(t.gameObject);

        List<ProfilePanelController.ProfileOwnedItemData> ownedList =
            parentController != null
            ? parentController.GetOwnedItemsByCategory(currentCategory)
            : null;

        if (ownedList == null || ownedList.Count == 0)
        {
            selectedItemId = -1;
            return;
        }

        foreach (var item in ownedList)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            // 여기서 DB의 itemId를 넘겨줌
            slot.Bind(item.itemId, item.icon, OnSlotSelected);
        }

        selectedItemId = -1;
    }

    private void OnSlotSelected(int id, ProfileSlot slot)
    {
        selectedItemId = id;

        foreach (Transform t in slotParent)
        {
            var s = t.GetComponent<ProfileSlot>();
            if (s != null)
                s.SetSelected(s == slot);
        }
    }

    private void Apply()
    {
        if (selectedItemId < 0)
            return;

        Sprite equippedSprite = parentController?.EquipItem(currentCategory, selectedItemId);

        // 아이콘 + id 둘 다 넘겨야 DB id가 안 꼬임
        if (equippedSprite != null && currentTargetIcon != null)
            currentTargetIcon.UpdateIcon(equippedSprite, selectedItemId);

        // Firebase 저장 그대로 유지
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            FirebaseDatabase.DefaultInstance
                .GetReference($"users/{user.UserId}/profile/equipped/{currentCategory}")
                .SetValueAsync(selectedItemId);
        }

        Close();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        currentCategory = null;
        currentTargetIcon = null;
        parentController = null;
        selectedItemId = -1;
    }
}