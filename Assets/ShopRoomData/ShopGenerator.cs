using System.Collections.Generic;
using UnityEngine;

public static class ShopGenerator
{
    public static List<ShopItemData> GenerateShopInventory(List<ShopItemData> itemPool, int itemCount)
    {
        List<ShopItemData> result = new List<ShopItemData>();

        if (itemPool == null || itemPool.Count == 0)
            return result;

        List<ShopItemData> shuffledPool = new List<ShopItemData>(itemPool);

        for (int i = 0; i < shuffledPool.Count; i++)
        {
            int swapIndex = Random.Range(i, shuffledPool.Count);
            ShopItemData temp = shuffledPool[i];
            shuffledPool[i] = shuffledPool[swapIndex];
            shuffledPool[swapIndex] = temp;
        }

        int finalCount = Mathf.Min(itemCount, shuffledPool.Count);

        for (int i = 0; i < finalCount; i++)
            result.Add(shuffledPool[i]);

        return result;
    }
}