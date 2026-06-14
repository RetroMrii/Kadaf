using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;
    [SerializeField] private HeroData secondHeroData;
    [SerializeField] private List<ShopItemData> initialShopItemPool = new List<ShopItemData>();
    public int roomsCleared = 0;
    public int healRoomsSeen = 0;
    public int lastHealRoomOfferedAt = -99;
    public int lastHealAmount = 0;
    public int recruitRoomsSeen = 0;
    public int lastRecruitRoomOfferedAt = -99;
    public int shopRoomsSeen = 0;
    public int lastShopRoomOfferedAt = -99;
    public int gold = 0;
    public List<ShopItemData> shopItemPool = new List<ShopItemData>();
    public List<ShopItemData> currentShopInventory = new List<ShopItemData>();
    public List<ShopItemData> ownedActivatedItems = new List<ShopItemData>();
    public List<ShopItemData> ownedRoundBuffItems = new List<ShopItemData>();
    public bool currentHealRoomUsed = false;
    public RoomData currentRoom;
    public RoomOfferSet currentOfferSet;
    public List<RunHeroState> party = new List<RunHeroState>();
    public HeroData recruitCandidateHero;
    public bool currentRecruitRoomUsed = false;

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
        shopItemPool = new List<ShopItemData>(initialShopItemPool);

        if (selectedHero != null)
        {
            party.Add(new RunHeroState
            {
                heroData = selectedHero,
                maxHP = selectedHero.maxHP,
                currentHP = selectedHero.maxHP,
                currentMana = 0,
                isAvailable = true
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
        AddGold(CombatRewards.lastCombatGoldReward);

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

    private void ResolveShopRoom()
    {
        GenerateShopInventory();
        SceneManager.LoadScene("ShopScene");
    }

    public void AddGold(int amount)
    {
        gold += Mathf.Max(0, amount);
    }

    public void GenerateShopInventory()
    {
        currentShopInventory = ShopGenerator.GenerateShopInventory(shopItemPool, 10);
    }

    public void LeaveShop()
    {
        roomsCleared++;
        GenerateNextOfferSet();
    }

    public bool TryBuyShopItem(ShopItemData item)
    {
        if (item == null)
            return false;

        if (gold < item.price)
            return false;

        gold -= item.price;
        ApplyShopItem(item);
        currentShopInventory.Remove(item);

        return true;
    }

    private void ApplyShopItem(ShopItemData item)
    {
        if (item == null)
            return;

        switch (item.itemType)
        {
            case ShopItemType.Heal:
                ApplyHealItem(item);
                break;

            case ShopItemType.RoundBuff:
                ApplyRoundBuffItem(item);
                break;

            case ShopItemType.ActivatedBuff:
                ApplyActivatedBuffItem(item);
                break;

            case ShopItemType.Revive:
                ApplyReviveItem(item);
                break;
        }
    }

    private void ApplyHealItem(ShopItemData item)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (!party[i].isAvailable)
                continue;

            int healAmount = Mathf.CeilToInt(party[i].maxHP * (item.healPercent / 100f));
            party[i].currentHP = Mathf.Min(party[i].currentHP + healAmount, party[i].maxHP);
        }
    }

    private void ApplyRoundBuffItem(ShopItemData item)
    {
        ownedRoundBuffItems.Add(item);
        Debug.Log($"Purchased round buff: {item.itemName}");
    }

    private void ApplyActivatedBuffItem(ShopItemData item)
    {
        ownedActivatedItems.Add(item);
        Debug.Log($"Purchased activated buff: {item.itemName}");
    }

    private void ApplyReviveItem(ShopItemData item)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (!party[i].isAvailable)
            {
                party[i].isAvailable = true;
                party[i].currentHP = 1;
                return;
            }
        }
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
            isAvailable = true
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

        if (roomsCleared >= 10)
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