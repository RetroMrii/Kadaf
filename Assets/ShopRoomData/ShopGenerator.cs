using System.Collections.Generic;
using UnityEngine;

public static class ShopGenerator
{
    public static List<ShopItemData> GenerateShopInventory(List<ShopItemData> itemPool, int itemCount)
    {
        List<ShopItemData> result = new List<ShopItemData>();

        if (itemPool == null || itemPool.Count == 0)
            return result;

        List<ShopItemData> availableItems = new List<ShopItemData>(itemPool);

        while (availableItems.Count > 0 && result.Count < itemCount)
        {
            int randomIndex = Random.Range(0, availableItems.Count);

            result.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }

        return result;
    }
}