using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public event EventHandler OnInventoryChanged;
    [SerializeField] private PlayerInventoryItem[] inventory = new PlayerInventoryItem[27];
    public PlayerInventoryItem[] GetInventory() => inventory;
    [SerializeField] private int maxQuantityPerSlot = 999;
    public int currentIndex = 0;
    public PlayerInventoryItem GetCurrentInvIndex()
    {
        if (currentIndex < 0 || currentIndex >= inventory.Length)
            return null;

        return inventory[currentIndex];
    }
    public void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= inventory.Length)
            return;

        currentIndex = index;
    }
    public void AddToInventory(string id, int quantity)
    {
        if (!Database.db.IsItemInCatalog(id))
            return;

        var gameItem = Database.db.GetGameItemByid(id);

        if (gameItem.stackable == 1)
        {
            for (int i = 0; i < inventory.Length && quantity > 0; i++)
            {
                var slot = inventory[i];
                if (slot != null && slot.id == id && slot.quantity < maxQuantityPerSlot)
                {
                    int canAdd = maxQuantityPerSlot - slot.quantity;
                    int toAdd = Mathf.Min(quantity, canAdd);
                    slot.quantity += toAdd;
                    quantity -= toAdd;
                }
            }

            for (int i = 0; i < inventory.Length && quantity > 0; i++)
            {
                if (inventory[i].id == string.Empty)
                {
                    int toAdd = Mathf.Min(quantity, maxQuantityPerSlot);
                    inventory[i] = new PlayerInventoryItem(id, toAdd);
                    quantity -= toAdd;
                }
            }
        }
        else
        {
            for (int i = 0; i < inventory.Length && quantity > 0; i++)
            {
                if (inventory[i].id == string.Empty)
                {
                    inventory[i] = new PlayerInventoryItem(id, 1);
                    quantity--;
                }
            }
        }

        // Nếu vẫn còn dư → nghĩa là túi đã full
        if (quantity > 0)
        {
            Debug.Log($"Inventory full, {quantity} item(s) could not be added!");
            // TODO: xử lý vứt ra ngoài map hoặc gửi vào storage
        }
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
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
