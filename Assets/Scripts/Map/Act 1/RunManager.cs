using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;
    [SerializeField] private HeroData secondHeroData;
    public int roomsCleared = 0;
    [SerializeField] private int roomsBeforeBoss = 10;
    public int healRoomsSeen = 0;
    public int lastHealRoomOfferedAt = -99;
    public int lastHealAmount = 0;
    public int recruitRoomsSeen = 0;
    public int lastRecruitRoomOfferedAt = -99;
    public int shopRoomsSeen = 0;
    public int lastShopRoomOfferedAt = -99;
    public bool currentHealRoomUsed = false;
    public RoomData currentRoom;
    public RoomOfferSet currentOfferSet;
    public List<RunHeroState> party = new List<RunHeroState>();
    public HeroData recruitCandidateHero;
    public bool currentRecruitRoomUsed = false;

    [Header("Economy")]
    public int gold = 0;

    [Header("Shop")]
    [SerializeField] private List<ShopItemData> initialShopItemPool = new List<ShopItemData>();

    public List<ShopItemData> shopItemPool = new List<ShopItemData>();
    public List<ShopItemData> currentShopInventory = new List<ShopItemData>();

    public List<ShopItemData> ownedBuffItems = new List<ShopItemData>();
    public List<ShopItemData> ownedKnowledgeItems = new List<ShopItemData>();
    public List<ShopItemData> ownedTechItems = new List<ShopItemData>();

    [Header("Knowledge Bonuses")]
    public bool hasXenoBiology = false;
    public bool hasSlyHands = false;
    public int xpRewardBonusPercent = 0;
    public int moneyRewardBonusPercent = 0;

    [SerializeField] private int maxOwnedTechItems = 2;
    public int MaxOwnedTechItems => maxOwnedTechItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRun(HeroData selectedHero)
    {
        currentRecruitRoomUsed = false;
        recruitCandidateHero = secondHeroData;
        roomsCleared = 0;
        healRoomsSeen = 0;
        lastHealRoomOfferedAt = -99;
        lastHealAmount = 0;
        recruitRoomsSeen = 0;
        lastRecruitRoomOfferedAt = -99;
        shopRoomsSeen = 0;
        lastShopRoomOfferedAt = -99;
        currentRoom = null;
        currentOfferSet = null;
        party.Clear();
        gold = 0;

        shopItemPool = new List<ShopItemData>(initialShopItemPool);
        currentShopInventory.Clear();

        ownedBuffItems.Clear();
        ownedKnowledgeItems.Clear();
        ownedTechItems.Clear();

        hasXenoBiology = false;
        hasSlyHands = false;
        xpRewardBonusPercent = 0;
        moneyRewardBonusPercent = 0;

        if (selectedHero != null)
        {
            party.Add(new RunHeroState
            {
                heroData = selectedHero,
                maxHP = selectedHero.maxHP,
                currentHP = selectedHero.maxHP,
                currentMana = 0,
                isAvailable = true,
                hasJoinedRun = true,
                currentXP = 0,
                level = 1,
                pendingStatAllocations = 0
            });
        }
    }

    public void BeginRun()
    {
        Debug.Log("begin run");
        currentOfferSet = new RoomOfferSet();
        currentOfferSet.options.Add(MapGenerator.GenerateFirstRoom());
        SceneManager.LoadScene("PickScene");
    }

    public void SelectRoom(RoomData room)
    {
        currentRoom = room;
        EnterCurrentRoom();
    }

    public void SavePartyStateFromCombat(HeroUnit[] heroes)
    {
        if (heroes == null || party.Count == 0)
            return;

        for (int i = 0; i < heroes.Length && i < party.Count; i++)
        {
            if (heroes[i] == null)
                continue;

            party[i].currentHP = heroes[i].CurrentHP;
            party[i].currentMana = heroes[i].currentMana;
            party[i].isAvailable = !heroes[i].IsDead;
        }
    }

    public void EnterCurrentRoom()
    {
        if (currentRoom == null)
        {
            Debug.LogWarning("No current room selected.");
            return;
        }

        switch (currentRoom.roomType)
        {
            case RoomType.Combat:
            case RoomType.Boss:
                StartCombat(currentRoom);
                break;

            case RoomType.Heal:
                currentHealRoomUsed = false;
                SceneManager.LoadScene("HealScene");
                break;

            case RoomType.Shop:
                GenerateShopInventory();
                SceneManager.LoadScene("ShopScene");
                break;

            case RoomType.Recruit:
                currentRecruitRoomUsed = false;
                SceneManager.LoadScene("RecruitScene");
                break;
        }
    }

    public void OnCombatFinished(HeroUnit[] heroes)
    {
        SavePartyStateFromCombat(heroes);

        int baseGoldReward = CombatRewards.lastCombatGoldReward;
        int finalGoldReward = GetModifiedGoldReward(baseGoldReward);
        AddGold(finalGoldReward);
        CombatRewards.lastCombatGoldReward = finalGoldReward;

        int baseXPReward = CombatRewards.lastCombatXPReward;
        int finalXPReward = GetModifiedXPReward(baseXPReward);
        AwardXPToParty(finalXPReward);
        CombatRewards.lastCombatXPReward = finalXPReward;

        if (currentRoom != null && currentRoom.roomType == RoomType.Boss)
        {
            OnRunWon();
            return;
        }

        roomsCleared++;
        GenerateNextOfferSet();
        SceneManager.LoadScene("PickScene");
    }

    public void OnRunFailed()
    {
        Debug.Log("Run failed.");
        SceneManager.LoadScene("MenuScene");
    }

    public void OnRunWon()
    {
        Debug.Log("Run won.");
        SceneManager.LoadScene("MenuScene");
    }

    public void SaveHeroState(HeroUnit hero)
    {
        if (hero == null || party.Count == 0)
            return;

        party[0].currentHP = hero.CurrentHP;
        party[0].currentMana = hero.currentMana;
        party[0].isAvailable = !hero.IsDead;

        Debug.Log($"Saved hero state: HP={party[0].currentHP}, Mana={party[0].currentMana}");
    }

    public void ResolveHealRoom()
    {
        if (currentHealRoomUsed)
            return;

        for (int i = 0; i < party.Count; i++)
        {
            if (!party[i].isAvailable)
                continue;

            int healAmount = Mathf.CeilToInt(party[i].maxHP * 0.3f);
            int oldHP = party[i].currentHP;
            party[i].currentHP = Mathf.Min(party[i].currentHP + healAmount, party[i].maxHP);
            lastHealAmount = party[i].currentHP - oldHP;
        }

        currentHealRoomUsed = true;
    }

    public void HealContinueClicked()
    {
        roomsCleared++;
        GenerateNextOfferSet();
        SceneManager.LoadScene("PickScene");
    }

    public int GetLimitedTechCount()
    {
        int count = 0;

        for (int i = 0; i < ownedTechItems.Count; i++)
        {
            if (ownedTechItems[i] != null && ownedTechItems[i].countsTowardTechLimit)
                count++;
        }

        return count;
    }

    public void AddGold(int amount)
    {
        gold += Mathf.Max(0, amount);
    }

    public int GetModifiedGoldReward(int baseAmount)
    {
        if (baseAmount <= 0)
            return 0;

        float multiplier = 1f + (moneyRewardBonusPercent / 100f);
        return Mathf.CeilToInt(baseAmount * multiplier);
    }

    public int GetModifiedXPReward(int baseAmount)
    {
        if (baseAmount <= 0)
            return 0;

        float multiplier = 1f + (xpRewardBonusPercent / 100f);
        return Mathf.CeilToInt(baseAmount * multiplier);
    }

    private void AwardXPToParty(int amount)
    {
        if (amount <= 0 || party == null || party.Count == 0)
            return;

        List<RunHeroState> availableHeroes = new List<RunHeroState>();

        for (int i = 0; i < party.Count; i++)
        {
            if (party[i] != null && party[i].isAvailable)
                availableHeroes.Add(party[i]);
        }

        if (availableHeroes.Count == 0)
            return;

        int xpPerHero = amount / availableHeroes.Count;
        int remainder = amount % availableHeroes.Count;

        for (int i = 0; i < availableHeroes.Count; i++)
        {
            int finalAmount = xpPerHero;

            if (i == 0)
                finalAmount += remainder;

            availableHeroes[i].currentXP += finalAmount;
        }
    }

    public void GenerateShopInventory()
    {
        currentShopInventory = ShopGenerator.GenerateShopInventory(shopItemPool, 10);
    }

    public bool TryBuyShopItem(ShopItemData item)
    {
        if (item == null)
            return false;

        if (gold < item.price)
            return false;

        if (item.itemType == ShopItemType.Tech && item.countsTowardTechLimit)
        {
            if (GetLimitedTechCount() >= maxOwnedTechItems)
                return false;
        }

        if (item.itemType == ShopItemType.Knowledge && ownedKnowledgeItems.Contains(item))
            return false;

        gold -= item.price;

        ApplyShopItem(item);

        currentShopInventory.Remove(item);

        if (item.itemType == ShopItemType.Knowledge || item.itemType == ShopItemType.Tech)
            shopItemPool.Remove(item);

        return true;
    }

    private void ApplyShopItem(ShopItemData item)
    {
        switch (item.itemType)
        {
            case ShopItemType.Booster:
                ApplyBoosterItem(item);
                break;

            case ShopItemType.Buff:
                ownedBuffItems.Add(item);
                break;

            case ShopItemType.Knowledge:
                ApplyKnowledgeItem(item);
                break;

            case ShopItemType.Tech:
                ownedTechItems.Add(item);
                break;
        }
    }

    private void ApplyBoosterItem(ShopItemData item)
    {
        switch (item.boosterType)
        {
            case BoosterType.Health:
                ApplyHealthBooster(item);
                break;

            case BoosterType.XP:
                AwardXPToParty(item.xpAmount);
                break;
        }
    }

    private void ApplyHealthBooster(ShopItemData item)
    {
        if (party == null)
            return;

        for (int i = 0; i < party.Count; i++)
        {
            if (!party[i].isAvailable)
                continue;

            int healAmount = Mathf.CeilToInt(party[i].maxHP * (item.healPercent / 100f));
            party[i].currentHP = Mathf.Min(party[i].maxHP, party[i].currentHP + healAmount);
        }
    }

    private void ApplyKnowledgeItem(ShopItemData item)
    {
        if (!ownedKnowledgeItems.Contains(item))
            ownedKnowledgeItems.Add(item);

        switch (item.knowledgeType)
        {
            case KnowledgeType.XenoBiology:
                hasXenoBiology = true;
                xpRewardBonusPercent = Mathf.Max(xpRewardBonusPercent, item.rewardBonusPercent);
                break;

            case KnowledgeType.SlyHands:
                hasSlyHands = true;
                moneyRewardBonusPercent = Mathf.Max(moneyRewardBonusPercent, item.rewardBonusPercent);
                break;
        }
    }

    public void LeaveShop()
    {
        roomsCleared++;
        GenerateNextOfferSet();
    }


    public void RecruitHero()
    {
        if (currentRecruitRoomUsed)
            return;

        if (party.Count >= 2)
            return;

        if (recruitCandidateHero == null)
            return;

        party.Add(new RunHeroState
        {
            heroData = recruitCandidateHero,
            maxHP = recruitCandidateHero.maxHP,
            currentHP = recruitCandidateHero.maxHP,
            currentMana = 0,
            isAvailable = true,
            hasJoinedRun = true,
            currentXP = 0,
            level = 1,
            pendingStatAllocations = 0
        });

        currentRecruitRoomUsed = true;
        roomsCleared++;
        GenerateNextOfferSet();
    }

    public void SkipRecruitRoom()
    {
        if (!currentRecruitRoomUsed)
            currentRecruitRoomUsed = true;

        roomsCleared++;
        GenerateNextOfferSet();
    }

    public void SkipHealRoom()
    {
        currentHealRoomUsed = true;
        roomsCleared++;
        GenerateNextOfferSet();
    }

    private void GenerateNextOfferSet()
    {
        currentRoom = null;

        if (roomsCleared >= roomsBeforeBoss)
        {
            currentOfferSet = new RoomOfferSet();
            currentOfferSet.options.Add(MapGenerator.GenerateBossRoom());
        }
        else
        {
            currentOfferSet = MapGenerator.GenerateRoomChoices(
                roomsCleared,
                healRoomsSeen, lastHealRoomOfferedAt,
                recruitRoomsSeen, lastRecruitRoomOfferedAt,
                shopRoomsSeen, lastShopRoomOfferedAt
            );
        }
    }

    private void StartCombat(RoomData room)
    {
        CombatState.currentEnemies = room.enemies;
        SceneManager.LoadScene("CombatScene");
    }
}