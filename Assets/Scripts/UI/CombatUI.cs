using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using TMPro;

public class CombatUI : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;
    public AbilityExecutor abilityExecutor;

    [Header("Main Text")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI heroStatsText;
    public TextMeshProUGUI alienStatsText;

    [Header("Buttons")]
    public Button abilityButton1;
    public Button abilityButton2;
    public Button abilityButton3;
    public Button endTurnButton;

    [Header("Button Labels")]
    public TextMeshProUGUI abilityButton1Text;
    public TextMeshProUGUI abilityButton2Text;
    public TextMeshProUGUI abilityButton3Text;
    public TextMeshProUGUI endTurnButtonText;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button continueButton;

    private bool lastCombatWon = false;

    //[Header("Scene Names")]
    //[SerializeField] private string mainMenuSceneName = "MenuScene";

    private void Start()
    {
        if (abilityButton1 != null)
            abilityButton1.onClick.AddListener(() => OnAbilityButtonPressed(0));

        if (abilityButton2 != null)
            abilityButton2.onClick.AddListener(() => OnAbilityButtonPressed(1));

        if (abilityButton3 != null)
            abilityButton3.onClick.AddListener(() => OnAbilityButtonPressed(2));

        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(OnEndTurnPressed);

        if (endTurnButtonText != null)
            endTurnButtonText.text = "End Turn";

        if (endPanel != null)
            endPanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (combatManager == null)
            return;

        HeroUnit hero = combatManager.GetActiveHero();
        AlienUnit alien = combatManager.GetFirstAliveAlien();

        RefreshPhaseText();
        RefreshHeroStats(hero);
        RefreshAlienStats(alien);
        RefreshAbilityButtons(hero);
        RefreshInteractableState(hero);
    }

    private void RefreshPhaseText()
    {
        if (phaseText == null || combatManager == null)
            return;

        if (combatManager.battleEnded)
        {
            if (combatManager.GetFirstAliveHero() == null)
                phaseText.text = "Defeat";
            else if (combatManager.GetFirstAliveAlien() == null)
                phaseText.text = "Victory";
            else
                phaseText.text = "Battle Ended";
        }
        else if (combatManager.playerTurn)
        {
            HeroUnit activeHero = combatManager.GetActiveHero();
            phaseText.text = activeHero != null
                ? $"Player Phase - {activeHero.UnitName}"
                : "Player Phase";
        }
        else
        {
            phaseText.text = "Alien Phase";
        }
    }

    private void RefreshHeroStats(HeroUnit hero)
    {
        if (heroStatsText == null)
            return;

        if (hero == null)
        {
            heroStatsText.text = "No Hero";
            return;
        }

        heroStatsText.text = hero.UnitName;

        if (hero.IsStatRevealed(StatType.HP))
            heroStatsText.text += $"\nHP: {hero.CurrentHP}/{hero.MaxHP}";

        if (hero.IsStatRevealed(StatType.Mana))
            heroStatsText.text += $"\nMana: {hero.currentMana}/{hero.maxMana}";

        string statsLine = "";

        if (hero.IsStatRevealed(StatType.Attack))
            statsLine += $"ATK: {hero.GetAttackValue()}";

        if (hero.IsStatRevealed(StatType.Defense))
        {
            if (statsLine.Length > 0)
                statsLine += "  ";

            statsLine += $"DEF: {hero.GetDefenseValue()}";
        }

        if (!string.IsNullOrEmpty(statsLine))
            heroStatsText.text += $"\n{statsLine}";
    }

    private void RefreshAlienStats(AlienUnit alien)
    {
        if (alienStatsText == null)
            return;

        if (alien == null)
        {
            alienStatsText.text = "No Alien";
            return;
        }

        alienStatsText.text = alien.UnitName;

        if (alien.IsStatRevealed(StatType.HP))
            alienStatsText.text += $"\nHP: {alien.CurrentHP}/{alien.MaxHP}";

        string statsLine = "";

        if (alien.IsStatRevealed(StatType.Attack))
            statsLine += $"ATK: {alien.GetAttackValue()}";

        if (alien.IsStatRevealed(StatType.Defense))
        {
            if (statsLine.Length > 0)
                statsLine += "  ";

            statsLine += $"DEF: {alien.GetDefenseValue()}";
        }

        if (!string.IsNullOrEmpty(statsLine))
            alienStatsText.text += $"\n{statsLine}";
    }

    private void RefreshAbilityButtons(HeroUnit hero)
    {
        AbilityData[] abilities = hero != null ? hero.GetAbilities() : null;

        SetAbilityLabel(abilityButton1Text, abilities, 0);
        SetAbilityLabel(abilityButton2Text, abilities, 1);
        SetAbilityLabel(abilityButton3Text, abilities, 2);
    }

    private void SetAbilityLabel(TextMeshProUGUI label, AbilityData[] abilities, int index)
    {
        if (label == null)
            return;

        if (abilities == null || index >= abilities.Length || abilities[index] == null)
        {
            label.text = "Empty";
            return;
        }

        AbilityData ability = abilities[index];
        label.text = $"{ability.abilityName}\nCost: {ability.manaCost}";
    }

    private void RefreshInteractableState(HeroUnit hero)
    {
        bool canAct = combatManager != null &&
                      combatManager.playerTurn &&
                      !combatManager.battleEnded &&
                      hero != null &&
                      !hero.IsDead;

        SetAbilityInteractable(abilityButton1, abilityButton1Text, hero, 0, canAct);
        SetAbilityInteractable(abilityButton2, abilityButton2Text, hero, 1, canAct);
        SetAbilityInteractable(abilityButton3, abilityButton3Text, hero, 2, canAct);

        if (endTurnButton != null)
            endTurnButton.interactable = canAct;
    }

    private void SetAbilityInteractable(Button button, TextMeshProUGUI label, HeroUnit hero, int index, bool canAct)
    {
        if (button == null)
            return;

        if (!canAct || hero == null)
        {
            button.interactable = false;
            if (label != null)
                label.alpha = 0.4f;
            return;
        }

        AbilityData[] abilities = hero.GetAbilities();
        if (abilities == null || index >= abilities.Length || abilities[index] == null)
        {
            button.interactable = false;
            if (label != null)
                label.alpha = 0.4f;
            return;
        }

        bool hasMana = hero.CanUseAbility(abilities[index]);
        button.interactable = hasMana;

        if (label != null)
            label.alpha = hasMana ? 1f : 0.4f;
    }

    private void OnAbilityButtonPressed(int index)
    {
        if (combatManager == null || abilityExecutor == null)
            return;

        if (!combatManager.playerTurn || combatManager.battleEnded)
            return;

        HeroUnit hero = combatManager.GetActiveHero();
        if (hero == null || hero.IsDead)
            return;

        AbilityData[] abilities = hero.GetAbilities();
        if (abilities == null || index < 0 || index >= abilities.Length)
            return;

        AbilityData ability = abilities[index];
        if (ability == null)
            return;

        if (!hero.CanUseAbility(ability))
        {
            combatManager.AddLog($"Failed to use {ability.abilityName}");
            RefreshUI();
            return;
        }

        if (ability.targetType == TargetType.AllEnemies)
        {
            UseAbilityOnAllEnemies(hero, ability);
            return;
        }

        BattleUnit target = GetTargetForAbility(ability, hero);
        if (target == null)
        {
            combatManager.AddLog($"No valid target for {ability.abilityName}");
            return;
        }

        int hpBefore = target.CurrentHP;

        bool castSucceeded = abilityExecutor.TryUseAbility(hero, ability, target);

        if (castSucceeded)
        {
            hero.UseMana(ability.manaCost);
        }

        if (!castSucceeded)
        {
            combatManager.AddLog($"Failed to use {ability.abilityName}");
            RefreshUI();
            return;
        }

        bool wasDodged = ability.abilityType == AbilityType.Damage &&
                         ability.canBeDodged &&
                         hpBefore == target.CurrentHP;

        LogSingleTargetAbility(hero, ability, target, hpBefore, wasDodged);

        RefreshUI();
        combatManager.CheckBattleEnd();

        if (combatManager.battleEnded)
            return;

        bool gainedExtraTurn = combatManager.heroExtraTurnAllowance > 0 &&
                               combatManager.ShouldGrantExtraTurn(hero);

        if (gainedExtraTurn)
        {
            combatManager.heroExtraTurnAllowance = 0;
            combatManager.AddLog($"{hero.UnitName} acts again!");
            RefreshUI();
            return;
        }

        combatManager.EndPlayerTurn();
    }

    private void OnContinueClicked()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogWarning("RunManager missing.");
            return;
        }
        
        if (lastCombatWon)
            RunManager.Instance.OnCombatFinished(combatManager.heroes);
        else
            RunManager.Instance.OnRunFailed();
    }


    private void UseAbilityOnAllEnemies(HeroUnit hero, AbilityData ability)
    {
        if (combatManager.aliens == null || combatManager.aliens.Length == 0)
        {
            combatManager.AddLog($"No valid targets for {ability.abilityName}");
            return;
        }

        bool hitAtLeastOne = false;
        bool manaSpent = false;

        for (int i = 0; i < combatManager.aliens.Length; i++)
        {
            AlienUnit alien = combatManager.aliens[i];

            if (alien == null || alien.IsDead)
                continue;

            int hpBefore = alien.CurrentHP;
            bool castSucceeded = abilityExecutor.TryUseAbility(hero, ability, alien);
            if (castSucceeded && !manaSpent)
            {
                hero.UseMana(ability.manaCost);
                manaSpent = true;
            }

            if (!castSucceeded)
                continue;

            bool wasDodged = ability.canBeDodged && hpBefore == alien.CurrentHP;

            if (wasDodged)
            {
                combatManager.AddLog(
                    $"{hero.UnitName} used {ability.abilityName} on {alien.UnitName}, but {alien.UnitName} dodged."
                );
            }
            else
            {
                int damageDealt = hpBefore - alien.CurrentHP;
                combatManager.AddLog(
                    $"{hero.UnitName} used {ability.abilityName} on {alien.UnitName} for {damageDealt} damage."
                );
            }

            hitAtLeastOne = true;
        }

        if (!hitAtLeastOne)
        {
            combatManager.AddLog($"No valid targets for {ability.abilityName}");
            RefreshUI();
            return;
        }

        RefreshUI();
        combatManager.CheckBattleEnd();

        if (combatManager.battleEnded)
            return;

        bool gainedExtraTurn = combatManager.heroExtraTurnAllowance > 0 &&
                               combatManager.ShouldGrantExtraTurn(hero);

        if (gainedExtraTurn)
        {
            combatManager.heroExtraTurnAllowance = 0;
            combatManager.AddLog($"{hero.UnitName} acts again!");
            RefreshUI();
            return;
        }

        combatManager.EndPlayerTurn();
    }

    private void LogSingleTargetAbility(HeroUnit hero, AbilityData ability, BattleUnit target, int hpBefore, bool wasDodged)
    {
        if (ability.abilityType == AbilityType.Damage)
        {
            if (wasDodged)
            {
                combatManager.AddLog(
                    $"{hero.UnitName} used {ability.abilityName} on {target.UnitName}, but {target.UnitName} dodged."
                );
            }
            else
            {
                int damageDealt = hpBefore - target.CurrentHP;
                combatManager.AddLog(
                    $"{hero.UnitName} used {ability.abilityName} on {target.UnitName} for {damageDealt} damage."
                );
            }
        }
        else if (ability.abilityType == AbilityType.Buff)
        {
            string atkText = ability.damageBuffAmount >= 0
                ? $"+{ability.damageBuffAmount}"
                : ability.damageBuffAmount.ToString();

            string defText = ability.defenseBuffAmount >= 0
                ? $"+{ability.defenseBuffAmount}"
                : ability.defenseBuffAmount.ToString();

            string effectWord = ability.isDebuff ? "inflicting" : "granting";

            combatManager.AddLog(
                $"{hero.UnitName} used {ability.abilityName} on {target.UnitName}, " +
                $"{effectWord} {atkText} ATK and {defText} DEF for {ability.buffDuration} turns."
            );
        }
        else if (ability.abilityType == AbilityType.Heal)
        {
            int healedAmount = target.CurrentHP - hpBefore;
            combatManager.AddLog(
                $"{hero.UnitName} used {ability.abilityName} on {target.UnitName} for {healedAmount} healing."
            );
        }
        else
        {
            combatManager.AddLog($"{hero.UnitName} used {ability.abilityName}.");
        }
    }

    private BattleUnit GetTargetForAbility(AbilityData ability, HeroUnit caster)
    {
        switch (ability.targetType)
        {
            case TargetType.Self:
                return caster;
            case TargetType.SingleEnemy:
                return combatManager.GetFirstAliveAlien();
            case TargetType.SingleAlly:
                return combatManager.GetFirstAliveHero();
            default:
                return null;
        }
    }

    private void OnEndTurnPressed()
    {
        if (combatManager == null)
            return;

        if (!combatManager.playerTurn || combatManager.battleEnded)
            return;

        combatManager.EndPlayerTurn();
        RefreshUI();
    }

    public void ShowEndPanel(bool playerWon)
    {
        lastCombatWon = playerWon;

        Debug.Log("ShowEndPanel called. Result = " + (playerWon ? "Victory" : "Defeat"));

        if (endPanel == null)
            return;

        endPanel.SetActive(true);

        if (resultText != null)
            resultText.text = playerWon ? "Victory" : "Defeat";
    }
}