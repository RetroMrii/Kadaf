using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text flavorText;
    [SerializeField] private TMP_Text healResultText;
    [SerializeField] private Button healButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (titleText != null)
            titleText.text = "HEAL";

        if (RunManager.Instance == null)
        {
            Debug.LogError("HealRoomUI: RunManager missing.");
            return;
        }

        RoomData room = RunManager.Instance.currentRoom;
        if (room != null)
        {
            if (flavorText != null)
                flavorText.text = room.flavorText;
        }

        if (healResultText != null && RunManager.Instance.party.Count > 0)
        {
            RunHeroState hero = RunManager.Instance.party[0];
            RefreshHealText();
        }

        if (healButton != null)
        {
            healButton.onClick.RemoveAllListeners();
            healButton.onClick.AddListener(OnHealClicked);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    private void OnHealClicked()
    {
        if (RunManager.Instance == null)
            return;

        if (RunManager.Instance.currentHealRoomUsed)
            return;

        RunManager.Instance.ResolveHealRoom();
        RefreshHealText();

        if (healButton != null)
            healButton.interactable = false;
    }

    private void OnContinueClicked()
    {
        if (RunManager.Instance == null)
            return;

        if (!RunManager.Instance.currentHealRoomUsed)
            RunManager.Instance.SkipHealRoom();

        RunManager.Instance.HealContinueClicked();
    }

    private void RefreshHealText()
    {
        if (healResultText == null || RunManager.Instance == null || RunManager.Instance.party.Count == 0)
            return;

        RunHeroState hero = RunManager.Instance.party[0];

        if (RunManager.Instance.currentHealRoomUsed && RunManager.Instance.lastHealAmount > 0)
        {
            healResultText.text = $"Recovered {RunManager.Instance.lastHealAmount} HP.\nCurrent HP: {hero.currentHP} / {hero.maxHP}";
        }
        else if (RunManager.Instance.currentHealRoomUsed)
        {
            healResultText.text = $"No healing was needed.\nCurrent HP: {hero.currentHP} / {hero.maxHP}";
        }
        else
        {
            healResultText.text = $"Current HP: {hero.currentHP} / {hero.maxHP}\nPress HEAL to recover health, or CONTINUE to move on.";
        }
    }
}