using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private List<PlayerInventoryItem> inventory;
    [SerializeField] private int maxQuantityPerSlot = 999;
    public void AddToInventory(string id, int quantity)
    {
        if (!Database.db.IsItemInCatalog(id))
            return;

        int currentQuantity = quantity;
        var gameItem = Database.db.GetGameItemByid(id);

        if (gameItem.stackable == 1) // Item có thể stack
        {
            var item = inventory.Find(e => e.id == id);
            if (item != null)
            {
                item.quantity += currentQuantity;

                while (item.quantity > maxQuantityPerSlot)
                {
                    int overflow = item.quantity - maxQuantityPerSlot;
                    item.quantity = maxQuantityPerSlot;
                    var newSlot = new PlayerInventoryItem(id, overflow);
                    inventory.Add(newSlot);
                    item = newSlot; // tiếp tục check slot mới
                }
            }
            else
            {
                while (currentQuantity > maxQuantityPerSlot)
                {
                    inventory.Add(new PlayerInventoryItem(id, maxQuantityPerSlot));
                    currentQuantity -= maxQuantityPerSlot;
                }
                inventory.Add(new PlayerInventoryItem(id, currentQuantity));
            }
        }
        else // Item không stack
        {
            inventory.Add(new PlayerInventoryItem(id, 1));
        }
    }

}
[Serializable]
public class PlayerInventoryItem
{
    public string id;
    public int quantity;

    public PlayerInventoryItem(string id, int currentQuantity)
    {
        this.id = id;
        quantity = currentQuantity;
    }
}