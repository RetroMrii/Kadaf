using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class HeroSelectManager : MonoBehaviour
{
    [Header("Hero Cards")]
    [SerializeField] private HeroCardUI[] heroCards;
    [SerializeField] private HeroCardUI defaultSelectedCard;

    [Header("Middle Info Panel")]
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text kitText;

    [Header("Side Overview Panel")]
    [SerializeField] private TMP_Text overviewText;

    [Header("Buttons")]
    [SerializeField] private Button startRunButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MenuScene";
    //[SerializeField] private string nextSceneName = "CombatScene";

    private HeroCardUI selectedCard;

    private void Start()
    {
        foreach (HeroCardUI card in heroCards)
        {
            if (card != null)
                card.Setup(this);
        }

        if (defaultSelectedCard != null && defaultSelectedCard.HeroData != null && defaultSelectedCard.IsUnlocked)
        {
            SelectHero(defaultSelectedCard);
        }
        else if (startRunButton != null)
        {
            startRunButton.interactable = false;
        }
    }

    public void PreviewHero(HeroCardUI card)
    {
        if (card == null || card.HeroData == null) return;
        UpdateUI(card.HeroData);
    }

    public void EndPreview()
    {
        if (selectedCard != null && selectedCard.HeroData != null)
            UpdateUI(selectedCard.HeroData);
    }

    public void SelectHero(HeroCardUI card)
    {
        if (card == null || card.HeroData == null) return;

        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = card;
        selectedCard.SetSelected(true);

        UpdateUI(selectedCard.HeroData);

        if (startRunButton != null)
            startRunButton.interactable = true;
    }

    private void UpdateUI(HeroData hero)
    {
        if (hero == null) return;

        if (heroNameText != null)
            heroNameText.text = hero.heroName;

        if (statsText != null)
        {
            statsText.text =
                $"HP: {hero.maxHP}\n" +
                $"BonusDMG: {hero.ATK}\n" +
                $"Defense: {hero.DFS}\n" +
                $"Mana: {hero.maxMana}\n" +
                $"Mana/Turn: {hero.manaPerTurn}\n" +
                $"Fire Resist: {hero.FRC}%\n" +
                $"Dodge: {hero.DOG}%\n" +
                $"Extra Turn: {hero.DCH}%";
        }

        if (kitText != null)
        {
            if (hero.abilities != null && hero.abilities.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hero.abilities.Length; i++)
                {
                    if (hero.abilities[i] != null)
                        sb.AppendLine($"• {hero.abilities[i].abilityName}");
                }

                kitText.text = sb.ToString();
            }
            else
            {
                kitText.text = "No abilities assigned.";
            }
        }

        if (overviewText != null)
            overviewText.text = hero.description;
    }

    public void StartRun()
    {
        if (selectedCard == null || selectedCard.HeroData == null)
            return;

        HeroSelectionState.SelectedHero = selectedCard.HeroData;

        if (RunManager.Instance == null)
        {
            Debug.LogError("No RunManager found.");
            return;
        }

        RunManager.Instance.StartRun(selectedCard.HeroData);
        RunManager.Instance.BeginRun();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}