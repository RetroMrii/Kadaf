using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text flavorText;
    [SerializeField] private Button continueButton;

    [Header("Item UI")]
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private TMP_Text[] itemNameTexts;
    [SerializeField] private TMP_Text[] itemPriceTexts;
    [SerializeField] private TMP_Text[] itemDescriptionTexts;

    private void Start()
    {
        if (titleText != null)
            titleText.text = "SHOP";

        if (RunManager.Instance == null)
        {
            Debug.LogError("ShopUI: RunManager missing.");
            return;
        }

        if (flavorText != null && RunManager.Instance.currentRoom != null)
            flavorText.text = RunManager.Instance.currentRoom.flavorText;

        RefreshUI();

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    private void RefreshUI()
    {
        if (RunManager.Instance == null)
            return;

        if (goldText != null)
            goldText.text = $"Gold: {RunManager.Instance.gold}";

        List<ShopItemData> items = RunManager.Instance.currentShopInventory;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (i >= items.Count)
            {
                itemButtons[i].gameObject.SetActive(false);
                continue;
            }

            ShopItemData item = items[i];
            itemButtons[i].gameObject.SetActive(true);

            if (itemNameTexts != null && i < itemNameTexts.Length && itemNameTexts[i] != null)
                itemNameTexts[i].text = item.itemName;

            if (itemPriceTexts != null && i < itemPriceTexts.Length && itemPriceTexts[i] != null)
                itemPriceTexts[i].text = $"{item.price}";

            if (itemDescriptionTexts != null && i < itemDescriptionTexts.Length && itemDescriptionTexts[i] != null)
                itemDescriptionTexts[i].text = item.description;

            itemButtons[i].onClick.RemoveAllListeners();
            itemButtons[i].onClick.AddListener(() => OnBuyClicked(item));
        }
    }

    private void OnBuyClicked(ShopItemData item)
    {
        if (RunManager.Instance == null)
            return;

        bool success = RunManager.Instance.TryBuyShopItem(item);
        if (success)
            RefreshUI();
    }

    private void OnContinueClicked()
    {
        if (RunManager.Instance == null)
            return;

        RunManager.Instance.LeaveShop();
        SceneManager.LoadScene("PickScene");
    }
}