using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public event EventHandler OnInventoryChanged;
    [SerializeField] private PlayerInventoryItem[] inventory = new PlayerInventoryItem[27];
    public PlayerInventoryItem[] GetInventory() => inventory;
    [SerializeField] private int maxQuantityPerSlot = 999;
    public int currentIndex = 0;
    [SerializeField] private int hotbarSize = 9;
    public int HotbarSize => hotbarSize;
    public PlayerInventoryItem GetCurrentInvIndex()
    {
        if (currentIndex < 0 || currentIndex >= hotbarSize)
            return null;
        return inventory[currentIndex];
    }
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Scroll.performed += OnScroll;
    }

    private void OnDisable()
    {
        controls.Player.Scroll.performed -= OnScroll;
        controls.Player.Disable();
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();
        float scrollY = scroll.y;

        if (scrollY > 0f)
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (scrollY < 0f)
        {
            currentIndex = Mathf.Min(HotbarSize - 1, currentIndex + 1);
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void OnHotbar1() => SetHotbar(0);
    public void OnHotbar2() => SetHotbar(1);
    public void OnHotbar3() => SetHotbar(2);
    public void OnHotbar4() => SetHotbar(3);
    public void OnHotbar5() => SetHotbar(4);
    public void OnHotbar6() => SetHotbar(5);
    public void OnHotbar7() => SetHotbar(6);
    public void OnHotbar8() => SetHotbar(7);
    public void OnHotbar9() => SetHotbar(8);
    private void SetHotbar(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, hotbarSize - 1);
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
    public PlayerInventoryItem GetItemByID(string id)
    {
        int totalQuantity = 0;
        foreach (var item in inventory)
        {
            if (item.id == id)
                totalQuantity += item.quantity;
        }
        return new PlayerInventoryItem(id, totalQuantity);
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
                    float durability = 0;
                    if (Database.db.GetGameItemByid(id).type == "weapon")
                    {
                        durability = gameItem.weaponData.wMaxDurability;
                    }
                    inventory[i] = new PlayerInventoryItem(id, 1, durability);
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
    public void DrainDurability(int index, float amount)
    {
        if (index < 0 || index >= inventory.Length)
            return;

        var slot = inventory[index];
        if (slot != null && slot.durability > 0)
        {
            slot.durability = Mathf.Max(0, slot.durability - amount);
        }
        if (slot.durability <= 0)
        {
            inventory[index] = new PlayerInventoryItem(string.Empty, 0);
        }
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
    public void RemoveFromInventory(string id, int quantity)
    {
        if (!Database.db.IsItemInCatalog(id))
            return;

        for (int i = 0; i < inventory.Length && quantity > 0; i++)
        {
            var slot = inventory[i];
            if (slot != null && slot.id == id && slot.quantity > 0)
            {
                int toRemove = Mathf.Min(quantity, slot.quantity);
                slot.quantity -= toRemove;
                quantity -= toRemove;

                if (slot.quantity <= 0)
                    inventory[i] = new PlayerInventoryItem(string.Empty, 0);
            }
        }

        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SwitchItemSlot(int index1, int index2)
    {
        if (index1 < 0 || index1 >= inventory.Length || index2 < 0 || index2 >= inventory.Length)
            return;
        Debug.Log($"Switching slot {index1} with slot {index2}");
        var temp = inventory[index1];
        inventory[index1] = inventory[index2];
        inventory[index2] = temp;
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
}

[Serializable]
public class PlayerInventoryItem
{
    public string id;
    public int quantity;
    public float durability;
    public float maxDurability;

    public PlayerInventoryItem(string id, int currentQuantity, float maxDurability = 0)
    {
        this.id = id;
        quantity = currentQuantity;
        this.maxDurability = maxDurability;
        durability = maxDurability;
    }
}
