using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecruitRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text flavorText;
    [SerializeField] private TMP_Text candidateNameText;
    [SerializeField] private TMP_Text candidateInfoText;
    [SerializeField] private Button recruitButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (titleText != null)
            titleText.text = "RECRUIT";

        if (RunManager.Instance == null)
        {
            Debug.LogError("RecruitRoomUI: RunManager missing.");
            return;
        }

        RoomData room = RunManager.Instance.currentRoom;
        if (room != null && flavorText != null)
            flavorText.text = room.flavorText;

        RefreshCandidateInfo();

        if (recruitButton != null)
        {
            recruitButton.onClick.RemoveAllListeners();
            recruitButton.onClick.AddListener(OnRecruitClicked);
            recruitButton.interactable = !RunManager.Instance.currentRecruitRoomUsed && RunManager.Instance.party.Count < 2;
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    private void RefreshCandidateInfo()
    {
        if (RunManager.Instance == null)
            return;

        HeroData hero = RunManager.Instance.recruitCandidateHero;
        if (hero == null)
        {
            if (candidateNameText != null)
                candidateNameText.text = "No Candidate";

            if (candidateInfoText != null)
                candidateInfoText.text = "No recruit is available.";
            return;
        }

        if (candidateNameText != null)
            candidateNameText.text = hero.heroName;

        if (candidateInfoText != null)
        {
            if (RunManager.Instance.currentRecruitRoomUsed)
            {
                candidateInfoText.text = $"{hero.heroName} joined your party.";
            }
            else if (RunManager.Instance.party.Count >= 2)
            {
                candidateInfoText.text = $"{hero.heroName} is here, but your party is full.";
            }
            else
            {
                candidateInfoText.text =
                    $"Bonus DMG: {hero.ATK}\n" +
                    $"Defense: {hero.DFS}\n" +
                    $"HP: {hero.maxHP}\n" +
                    $"Mana: {hero.maxMana}\n" +
                    $"{hero.description}";
            }
        }
    }

    private void OnRecruitClicked()
    {
        Debug.Log("Recruit button clicked.");
        if (RunManager.Instance == null)
            return;

        if (RunManager.Instance.currentRecruitRoomUsed)
            return;

        RunManager.Instance.RecruitHero();
        RefreshCandidateInfo();

        if (recruitButton != null)
            recruitButton.interactable = false;
    }

    private void OnContinueClicked()
    {
        Debug.Log("Continue button clicked.");
        if (RunManager.Instance == null)
            return;

        if (!RunManager.Instance.currentRecruitRoomUsed)
            RunManager.Instance.SkipRecruitRoom();

        SceneManager.LoadScene("PickScene");
    }
}
