using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Main Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Shop Slots - Size 10")]
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private TMP_Text[] itemNameTexts;
    [SerializeField] private TMP_Text[] itemPriceTexts;
    [SerializeField] private TMP_Text[] itemDescriptionTexts;
    [SerializeField] private TMP_Text[] itemTypeTexts;

    private void Start()
    {
        if (titleText != null)
            titleText.text = "TRAITOR SHOP";

        if (feedbackText != null)
            feedbackText.text = "";

        if (RunManager.Instance != null && RunManager.Instance.currentShopInventory.Count == 0)
            RunManager.Instance.GenerateShopInventory();

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (RunManager.Instance == null)
            return;

        if (goldText != null)
            goldText.text = $"Credits: {RunManager.Instance.gold}";

        int slotCount = itemButtons != null ? itemButtons.Length : 0;

        for (int i = 0; i < slotCount; i++)
        {
            bool hasItem = i < RunManager.Instance.currentShopInventory.Count;
            ShopItemData item = hasItem ? RunManager.Instance.currentShopInventory[i] : null;

            if (itemButtons[i] != null)
            {
                itemButtons[i].gameObject.SetActive(hasItem);
                itemButtons[i].onClick.RemoveAllListeners();

                if (hasItem)
                {
                    bool canAfford = RunManager.Instance.gold >= item.price;
                    bool techLimitReached = item.itemType == ShopItemType.Tech &&
                                            item.countsTowardTechLimit &&
                                            RunManager.Instance.GetLimitedTechCount() >= RunManager.Instance.MaxOwnedTechItems;

                    bool alreadyOwnsKnowledge = item.itemType == ShopItemType.Knowledge &&
                                                RunManager.Instance.ownedKnowledgeItems.Contains(item);

                    itemButtons[i].interactable = canAfford && !techLimitReached && !alreadyOwnsKnowledge;

                    ShopItemData capturedItem = item;
                    itemButtons[i].onClick.AddListener(() => OnBuyClicked(capturedItem));
                }
                else
                {
                    itemButtons[i].interactable = false;
                }
            }

            if (itemNameTexts != null && i < itemNameTexts.Length && itemNameTexts[i] != null)
                itemNameTexts[i].text = hasItem ? item.itemName : "";

            if (itemPriceTexts != null && i < itemPriceTexts.Length && itemPriceTexts[i] != null)
                itemPriceTexts[i].text = hasItem ? $"{item.price} Credits" : "";

            if (itemDescriptionTexts != null && i < itemDescriptionTexts.Length && itemDescriptionTexts[i] != null)
                itemDescriptionTexts[i].text = hasItem ? item.description : "";

            if (itemTypeTexts != null && i < itemTypeTexts.Length && itemTypeTexts[i] != null)
                itemTypeTexts[i].text = hasItem ? item.itemType.ToString() : "";
        }
    }

    private void OnBuyClicked(ShopItemData item)
    {
        if (RunManager.Instance == null || item == null)
            return;

        bool bought = RunManager.Instance.TryBuyShopItem(item);

        if (feedbackText != null)
        {
            if (bought)
                feedbackText.text = $"Bought {item.itemName}.";
            else
                feedbackText.text = GetFailureMessage(item);
        }

        RefreshUI();
    }

    private string GetFailureMessage(ShopItemData item)
    {
        if (RunManager.Instance == null || item == null)
            return "Cannot buy item.";

        if (RunManager.Instance.gold < item.price)
            return "Not enough credits.";

        if (item.itemType == ShopItemType.Tech &&
            item.countsTowardTechLimit &&
            RunManager.Instance.GetLimitedTechCount() >= RunManager.Instance.MaxOwnedTechItems)
        {
            return "Tech limit reached.";
        }

        if (item.itemType == ShopItemType.Knowledge && RunManager.Instance.ownedKnowledgeItems.Contains(item))
            return "Knowledge already owned.";

        return "Cannot buy item.";
    }

    private void OnContinueClicked()
    {
        if (RunManager.Instance != null)
            RunManager.Instance.LeaveShop();

        SceneManager.LoadScene("PickScene");
    }
}