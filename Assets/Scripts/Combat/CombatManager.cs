using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Combatants")]
    public HeroUnit[] heroes;
    public AlienUnit[] aliens;

    [Header("UI")]
    public CombatLogUI combatLogUI;
    [SerializeField] private CombatUI combatUI;

    [Header("Enemy Database")]
    [SerializeField] private AlienData gruntData;
    [SerializeField] private AlienData assassinData;
    [SerializeField] private AlienData droneData;
    [SerializeField] private AlienData heavyData;
    [SerializeField] private AlienData warlockData;

    [Header("Turn State")]
    public int activeHeroIndex = 0;
    public bool playerTurn = false;
    public bool battleEnded = false;

    [Header("Timing")]
    public float alienActionDelay = 0.6f;

    private Coroutine turnRoutine;
    public int heroExtraTurnAllowance = 0;

    private void Start()
    {
        StartBattle();
    }

    private void LoadRoomEnemies()
    {
        if (aliens == null || aliens.Length == 0)
            return;

        if (CombatState.currentEnemies == null || CombatState.currentEnemies.Length == 0)
        {
            Debug.LogWarning("LoadRoomEnemies: CombatState.currentEnemies is empty.");
            return;
        }

        for (int i = 0; i < aliens.Length; i++)
        {
            if (aliens[i] == null)
                continue;

            if (i >= CombatState.currentEnemies.Length)
            {
                aliens[i].gameObject.SetActive(false);
                continue;
            }

            AlienData data = GetAlienDataForType(CombatState.currentEnemies[i]);

            if (data == null)
            {
                Debug.LogError($"No AlienData assigned for enemy type {CombatState.currentEnemies[i]}.");
                aliens[i].gameObject.SetActive(false);
                continue;
            }

            aliens[i].gameObject.SetActive(true);
            aliens[i].alienData = data;
            aliens[i].InitializeFromAssignedData();
            Debug.Log($"Assigned {data.alienName} to alien slot {i}");
        }
    }

    private AlienData GetAlienDataForType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Grunt:
                return gruntData;
            case EnemyType.Assassin:
                return assassinData;
            case EnemyType.Drone:
                return droneData;
            case EnemyType.Heavy:
                return heavyData;
            case EnemyType.Warlock:
                return warlockData;
            default:
                return null;
        }
    }

    public void StartBattle()
    {
        LoadPartyFromRunState();
        LoadRoomEnemies();
        CombatRewards.lastCombatGoldReward = CombatRewards.CalculateGoldReward(CombatState.currentEnemies);
        CombatRewards.lastCombatXPReward = CombatRewards.CalculateXPReward(CombatState.currentEnemies);

        battleEnded = false;
        playerTurn = false;
        activeHeroIndex = 0;

        StartPlayerTurn();
    }

    private void LoadSelectedHero()
    {
        if (heroes == null || heroes.Length == 0 || heroes[0] == null)
            return;

        if (RunManager.Instance != null && RunManager.Instance.party.Count > 0)
        {
            RunHeroState state = RunManager.Instance.party[0];

            if (state != null && state.heroData != null)
            {
                heroes[0].heroData = state.heroData;
                heroes[0].InitializeHero();
                heroes[0].RestorePersistentState(state.currentHP, state.currentMana);

                Debug.Log($"Loaded hero state: HP={state.currentHP}, Mana={state.currentMana}");
                return;
            }
        }

        if (HeroSelectionState.SelectedHero != null)
        {
            heroes[0].heroData = HeroSelectionState.SelectedHero;
            heroes[0].InitializeHero();
            Debug.Log("Loaded fallback hero from HeroSelectionState.");
        }
    }


    private void LoadPartyFromRunState()
    {
        if (heroes == null || heroes.Length == 0)
            return;

        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] != null)
                heroes[i].gameObject.SetActive(false);
        }

        if (RunManager.Instance == null || RunManager.Instance.party.Count == 0)
        {
            LoadSelectedHero();
            return;
        }

        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] == null)
                continue;

            if (i >= RunManager.Instance.party.Count)
            {
                heroes[i].gameObject.SetActive(false);
                continue;
            }

            RunHeroState state = RunManager.Instance.party[i];
            if (state == null || state.heroData == null || !state.isAvailable)
            {
                heroes[i].gameObject.SetActive(false);
                continue;
            }

            heroes[i].gameObject.SetActive(true);
            heroes[i].heroData = state.heroData;
            heroes[i].InitializeHero();
            heroes[i].RestorePersistentState(state.currentHP, state.currentMana);
        }
    }

    public void AddLog(string message)
    {
        if (combatLogUI != null)
            combatLogUI.AddEntry(message);

        Debug.Log(message);
    }

    public HeroUnit GetActiveHero()
    {
        if (heroes == null || heroes.Length == 0)
            return null;

        if (activeHeroIndex < 0 || activeHeroIndex >= heroes.Length)
            return null;

        return heroes[activeHeroIndex];
    }

    public HeroUnit GetFirstAliveHero()
    {
        if (heroes == null || heroes.Length == 0)
            return null;

        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i] != null && !heroes[i].IsDead)
                return heroes[i];
        }

        return null;
    }

    public AlienUnit GetFirstAliveAlien()
    {
        if (aliens == null || aliens.Length == 0)
            return null;

        for (int i = 0; i < aliens.Length; i++)
        {
            if (aliens[i] != null && !aliens[i].IsDead)
                return aliens[i];
        }

        return null;
    }

    public void StartPlayerTurn()
    {
        if (battleEnded)
            return;

        HeroUnit activeHero = GetActiveHero();

        if (activeHero == null)
        {
            CheckBattleEnd();
            return;
        }

        if (activeHero.IsDead)
        {
            AdvanceToNextLivingHeroOrEnd();
            return;
        }

        playerTurn = true;
        heroExtraTurnAllowance = 1;
        activeHero.StartTurn();
        ApplyHeroPassiveReveals();
    }

    private void ApplyHeroPassiveReveals()
    {
        HeroUnit activeHero = GetActiveHero();
        if (activeHero == null)
            return;

        if (!activeHero.HasPassive(HeroPassiveType.SizeYouUp))
            return;

        if (aliens == null)
            return;

        for (int i = 0; i < aliens.Length; i++)
        {
            if (aliens[i] != null && !aliens[i].IsDead)
            {
                aliens[i].RevealStat(StatType.ATK);
                aliens[i].RevealStat(StatType.DFS);
            }
        }
    }

    public void EndPlayerTurn()
    {
        if (battleEnded)
            return;

        if (!playerTurn)
            return;

        playerTurn = false;

        if (turnRoutine != null)
            StopCoroutine(turnRoutine);

        turnRoutine = StartCoroutine(AlienTurnRoutine());
    }

    public bool ShouldGrantExtraTurn(BattleUnit unit)
    {
        if (unit == null)
            return false;

        return Random.Range(0, 100) < unit.ExtraTurnChancePercent;
    }

    public int GetExtraTurnAllowance(BattleUnit unit)
    {
        if (unit == null)
            return 0;

        return ShouldGrantExtraTurn(unit) ? 1 : 0;
    }

    private IEnumerator AlienTurnRoutine()
    {
        if (battleEnded)
            yield break;

        HeroUnit targetHero = GetFirstAliveHero();
        if (targetHero == null)
        {
            CheckBattleEnd();
            yield break;
        }

        if (aliens == null || aliens.Length == 0)
        {
            CheckBattleEnd();
            yield break;
        }

        bool firstAlienAction = true;

        for (int i = 0; i < aliens.Length; i++)
        {
            AlienUnit alien = aliens[i];

            if (alien == null || alien.IsDead)
                continue;

            targetHero = GetFirstAliveHero();
            if (targetHero == null)
            {
                CheckBattleEnd();
                yield break;
            }

            if (!firstAlienAction)
                yield return new WaitForSeconds(alienActionDelay);

            alien.ProcessStatusEffects();

            int extraTurnAllowance = 0;
            bool firstAction = true;

            do
            {
                if (!firstAction)
                {
                    AddLog($"{alien.UnitName} acts again!");
                    yield return new WaitForSeconds(alienActionDelay);
                }

                targetHero = GetFirstAliveHero();
                if (targetHero == null)
                {
                    CheckBattleEnd();
                    yield break;
                }

                alien.TakeTurn(targetHero, this);

                CheckBattleEnd();
                if (battleEnded)
                    yield break;

                if (firstAction)
                {
                    extraTurnAllowance = GetExtraTurnAllowance(alien);
                    firstAction = false;
                }
                else
                {
                    extraTurnAllowance = 0;
                }

            } while (extraTurnAllowance > 0);

            firstAlienAction = false;
        }

        AdvanceTurnOrder();
        StartPlayerTurn();
    }

    private void AdvanceTurnOrder()
    {
        if (heroes == null || heroes.Length == 0)
            return;

        int safety = heroes.Length;

        do
        {
            activeHeroIndex++;
            if (activeHeroIndex >= heroes.Length)
                activeHeroIndex = 0;

            HeroUnit hero = heroes[activeHeroIndex];

            if (hero != null && !hero.IsDead)
                return;

            safety--;
        }
        while (safety > 0);

        CheckBattleEnd();
    }

    private void AdvanceToNextLivingHeroOrEnd()
    {
        if (heroes == null || heroes.Length == 0)
        {
            CheckBattleEnd();
            return;
        }

        int originalIndex = activeHeroIndex;

        do
        {
            activeHeroIndex++;
            if (activeHeroIndex >= heroes.Length)
                activeHeroIndex = 0;

            HeroUnit hero = heroes[activeHeroIndex];
            if (hero != null && !hero.IsDead)
            {
                StartPlayerTurn();
                return;
            }
        }
        while (activeHeroIndex != originalIndex);

        CheckBattleEnd();
    }

    public void CheckBattleEnd()
    {
        if (battleEnded)
            return;

        bool anyHeroAlive = false;
        if (heroes != null)
        {
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] != null && !heroes[i].IsDead)
                {
                    anyHeroAlive = true;
                    break;
                }
            }
        }

        bool anyAlienAlive = false;
        if (aliens != null)
        {
            for (int i = 0; i < aliens.Length; i++)
            {
                if (aliens[i] != null && !aliens[i].IsDead)
                {
                    anyAlienAlive = true;
                    break;
                }
            }
        }

        if (!anyHeroAlive)
        {
            EndCombat(false);
            return;
        }

        if (!anyAlienAlive)
        {
            EndCombat(true);
            return;
        }
    }

    private void EndCombat(bool playerWon)
    {
        if (battleEnded)
            return;

        battleEnded = true;
        playerTurn = false;

        AddLog(playerWon ? "Victory!" : "Defeat!");

        if (combatUI != null)
        {
            combatUI.ShowEndPanel(playerWon);
        }
        else
        {
            Debug.LogWarning("CombatUI not assigned in CombatManager.");
        }
    }
}