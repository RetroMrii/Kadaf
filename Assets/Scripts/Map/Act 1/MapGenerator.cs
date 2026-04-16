using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static RoomData GenerateFirstRoom()
    {
        return new RoomData
        {
            roomType = RoomType.Combat,
            enemies = new EnemyType[] { EnemyType.Grunt, EnemyType.Grunt },
            flavorText = "A dim security corridor. Footsteps echo ahead."
        };
    }

    public static RoomOfferSet GenerateRoomChoices(
        int roomsCleared,
        int healRoomsSeen, int lastHealRoomOfferedAt,
        int recruitRoomsSeen, int lastRecruitRoomOfferedAt,
        int shopRoomsSeen, int lastShopRoomOfferedAt
    )
    {
        RoomOfferSet offerSet = new RoomOfferSet();

        HashSet<string> usedCombatSignatures = new HashSet<string>();

        bool healAlreadyUsedInSet = false;
        bool recruitAlreadyUsedInSet = false;
        bool shopAlreadyUsedInSet = false;

        for (int i = 0; i < 3; i++)
        {
            RoomData room = GenerateSingleRoomOption(
                roomsCleared,
                healRoomsSeen, lastHealRoomOfferedAt,
                recruitRoomsSeen, lastRecruitRoomOfferedAt,
                shopRoomsSeen, lastShopRoomOfferedAt,
                healAlreadyUsedInSet,
                recruitAlreadyUsedInSet,
                shopAlreadyUsedInSet
            );

            int safety = 20;
            while (room.roomType == RoomType.Combat && safety > 0)
            {
                string signature = GetCombatSignature(room.enemies);

                if (!usedCombatSignatures.Contains(signature))
                {
                    usedCombatSignatures.Add(signature);
                    break;
                }

                room = GenerateSingleRoomOption(
                    roomsCleared,
                    healRoomsSeen, lastHealRoomOfferedAt,
                    recruitRoomsSeen, lastRecruitRoomOfferedAt,
                    shopRoomsSeen, lastShopRoomOfferedAt,
                    healAlreadyUsedInSet,
                    recruitAlreadyUsedInSet,
                    shopAlreadyUsedInSet
                );

                safety--;
            }

            if (room.roomType == RoomType.Combat)
                usedCombatSignatures.Add(GetCombatSignature(room.enemies));

            if (room.roomType == RoomType.Heal)
                healAlreadyUsedInSet = true;

            if (room.roomType == RoomType.Recruit)
                recruitAlreadyUsedInSet = true;

            if (room.roomType == RoomType.Shop)
                shopAlreadyUsedInSet = true;

            offerSet.options.Add(room);
        }

        bool hasHeal = false;
        bool hasRecruit = false;
        bool hasShop = false;

        for (int i = 0; i < offerSet.options.Count; i++)
        {
            RoomType type = offerSet.options[i].roomType;

            if (type == RoomType.Heal)
                hasHeal = true;

            if (type == RoomType.Recruit)
                hasRecruit = true;

            if (type == RoomType.Shop)
                hasShop = true;
        }

        if (RunManager.Instance != null)
        {
            if (hasHeal)
            {
                RunManager.Instance.healRoomsSeen++;
                RunManager.Instance.lastHealRoomOfferedAt = roomsCleared;
            }

            if (hasRecruit)
            {
                RunManager.Instance.recruitRoomsSeen++;
                RunManager.Instance.lastRecruitRoomOfferedAt = roomsCleared;
            }

            if (hasShop)
            {
                RunManager.Instance.shopRoomsSeen++;
                RunManager.Instance.lastShopRoomOfferedAt = roomsCleared;
            }
        }

        return offerSet;
    }

    public static RoomData GenerateBossRoom()
    {
        return new RoomData
        {
            roomType = RoomType.Boss,
            enemies = new EnemyType[] { EnemyType.Warlock, EnemyType.Heavy },
            flavorText = "At the heart of the structure, something waits."
        };
    }

    private static RoomData GenerateSingleRoomOption(
        int roomsCleared,
        int healRoomsSeen, int lastHealRoomOfferedAt,
        int recruitRoomsSeen, int lastRecruitRoomOfferedAt,
        int shopRoomsSeen, int lastShopRoomOfferedAt,
        bool healAlreadyUsedInSet,
        bool recruitAlreadyUsedInSet,
        bool shopAlreadyUsedInSet
    )
    {
        List<RoomType> possibleTypes = new List<RoomType>
        {
            RoomType.Combat
        };

        bool healAllowed =
            roomsCleared >= 2 &&
            healRoomsSeen < 2 &&
            (roomsCleared - lastHealRoomOfferedAt) >= 3 &&
            !healAlreadyUsedInSet;

        if (healAllowed)
            possibleTypes.Add(RoomType.Heal);

        bool recruitAllowed =
            roomsCleared >= 5 &&
            recruitRoomsSeen < 2 &&
            (roomsCleared - lastRecruitRoomOfferedAt) >= 3 &&
            !recruitAlreadyUsedInSet;

        if (recruitAllowed)
            possibleTypes.Add(RoomType.Recruit);

        bool shopAllowed =
            roomsCleared >= 6 &&
            shopRoomsSeen < 2 &&
            (roomsCleared - lastShopRoomOfferedAt) >= 3 &&
            !shopAlreadyUsedInSet;

        if (shopAllowed)
            possibleTypes.Add(RoomType.Shop);

        RoomType chosenType = possibleTypes[Random.Range(0, possibleTypes.Count)];

        RoomData room = new RoomData
        {
            roomType = chosenType,
            flavorText = GetFlavorText(chosenType)
        };

        if (chosenType == RoomType.Combat)
            room.enemies = GenerateEnemies();

        return room;
    }

    private static EnemyType[] GenerateEnemies()
    {
        EnemyType first = GetRandomEnemy();
        EnemyType second = GetValidSecondEnemy(first);

        return new EnemyType[] { first, second };
    }

    private static EnemyType GetRandomEnemy()
    {
        int roll = Random.Range(0, 5);
        return (EnemyType)roll;
    }

    private static EnemyType GetValidSecondEnemy(EnemyType first)
    {
        List<EnemyType> possible = new List<EnemyType>
        {
            EnemyType.Grunt,
            EnemyType.Assassin,
            EnemyType.Drone,
            EnemyType.Heavy,
            EnemyType.Warlock
        };

        if (first == EnemyType.Heavy || first == EnemyType.Warlock)
            possible.Remove(first);

        return possible[Random.Range(0, possible.Count)];
    }

    private static string GetCombatSignature(EnemyType[] enemies)
    {
        if (enemies == null || enemies.Length == 0)
            return "NONE";

        if (enemies.Length == 1)
            return enemies[0].ToString();

        string a = enemies[0].ToString();
        string b = enemies[1].ToString();

        return string.CompareOrdinal(a, b) <= 0
            ? $"{a}_{b}"
            : $"{b}_{a}";
    }

    private static string GetFlavorText(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Heal:
                return GetRandomFrom(
                    "A silent lab hums with sterile light.",
                    "Rows of cracked medbeds flicker faintly.",
                    "A recovery bay glows behind sealed glass."
                );

            case RoomType.Recruit:
                return GetRandomFrom(
                    "A prison block groans somewhere in the dark.",
                    "A holding wing lies behind reinforced doors.",
                    "You hear movement from inside a containment cell."
                );

            case RoomType.Shop:
                return GetRandomFrom(
                    "A scavenger's stall has been set up in the wreckage.",
                    "Someone has turned an access bay into a market.",
                    "A black-market cache is hidden beyond a service hatch."
                );

            case RoomType.Combat:
                return GetRandomFrom(
                    "A dim corridor stretches ahead.",
                    "An active checkpoint blocks the way.",
                    "An exposed research hall lies open and dangerous.",
                    "A cargo transit lane rattles with distant motion.",
                    "A broken security wing flickers under red lights."
                );

            case RoomType.Boss:
                return "A pressure hangs in the air. Something important is near.";

            default:
                return "An unfamiliar chamber lies ahead.";
        }
    }

    private static string GetRandomFrom(params string[] values)
    {
        return values[Random.Range(0, values.Length)];
    }
}