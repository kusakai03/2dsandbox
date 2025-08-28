using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTex : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI coordinate;
    [SerializeField] private GameObject[] itemSlotsUI;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject[] inventoryItemSlotsUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject[] invPages;
    [SerializeField] private RectTransform recipesContent;
    [SerializeField] private GameObject recipesItem;
    [SerializeField] private GameObject[] recipeMaterialItem;
    [SerializeField] private GameObject productItem;
    [SerializeField] private CraftingRecipe craftingRecipe;
    private int currentPage = 0;
    [SerializeField] private int craftingStrength;
    [SerializeField] private GameObject craftingButton;
    private int selectedRecipeIndex = -1;
    private List<Craft> recipes = new();
    private void Start()
    {
        PlayerData playerData = player.GetComponent<PlayerData>();
        playerData.OnInventoryChanged += PlayerData_OnInventoryChanged;
        PlayerAction playerAction = player.GetComponent<PlayerAction>();
        playerAction.OnPlayerOpenInventory += PlayerAction_OnPlayerOpenInventory;
        UpdateItemSlotsUI();
        for (int i = 0; i < inventoryItemSlotsUI.Length; i++)
        {
            var slotObj = inventoryItemSlotsUI[i];
            var slot = slotObj.GetComponent<InventorySlot>();
            slot.SetMaster(i, player);
        }
        recipes = new();
        selectedRecipeIndex = -1;
    }
    public void Exit()
    {
        inventoryUI.SetActive(false);
    }
    private void PlayerAction_OnPlayerOpenInventory(object sender, EventArgs e)
    {
        if (inventoryUI.activeInHierarchy) return;
        UpdatePlayerInventory();
        UpdateCraftingPage();
        inventoryUI.SetActive(true);
        SetPage(0);
    }
    public void SetPage(int page)
    {
        currentPage = page;
        foreach (var i in invPages) i.SetActive(false);
        invPages[page].SetActive(true);
    }
    private void UpdatePlayerInventory()
    {
        var inventory = player.GetComponent<PlayerData>().GetInventory();
        for (int i = 0; i < inventoryItemSlotsUI.Length; i++)
        {
            var slotObj = inventoryItemSlotsUI[i];
            var slot = slotObj.GetComponent<InventorySlot>();
            slot.slotIndex = i;

            var item = inventory[i];
            if (item != null && item.id != string.Empty)
            {
                slot.SetItem(Database.db.GetGameItemByid(item.id).iSpr, item.quantity);
            }
            else
            {
                slot.SetItem(null, 0);
            }
        }
    }
    private void UpdateCraftingPage()
    {
        recipes = new();
        foreach (Transform child in recipesContent)
        {
            child.GetComponent<SelectItem>().onSelectItem -= OnSelectRecipe;
            Destroy(child.gameObject);
        }
        int number = 0;
        foreach (var craft in craftingRecipe.crafts)
        {
            if (craft.craftStrength > craftingStrength) continue;
            if (craft.productID != string.Empty)
            {
                recipes.Add(craft);
                var recipeObj = Instantiate(recipesItem, recipesContent);
                recipeObj.SetActive(true);
                recipeObj.GetComponent<SelectItem>().onSelectItem += OnSelectRecipe;
                recipeObj.GetComponent<SelectItem>().SetIndex(number);
                recipeObj.GetComponentInChildren<Image>().sprite = Database.db.GetGameItemByid(craft.productID).iSpr;
                number++;
            }
        }
        if (selectedRecipeIndex != -1)
        {
            Craft c = recipes[selectedRecipeIndex];
            for (int i = 0; i < recipeMaterialItem.Length; i++)
            {
                if (i < c.materials.Count)
                {
                    var mat = c.materials[i];
                    recipeMaterialItem[i].SetActive(true);
                    recipeMaterialItem[i].GetComponentInChildren<Image>().sprite = Database.db.GetGameItemByid(mat.iID).iSpr;
                    recipeMaterialItem[i].GetComponentInChildren<TextMeshProUGUI>().text = mat.amount.ToString() + " / " +
                        player.GetComponent<PlayerData>().GetItemByID(mat.iID)?.quantity.ToString();
                }
                else
                {
                    recipeMaterialItem[i].SetActive(false);
                }
            }
            productItem.GetComponentInChildren<Image>().sprite = Database.db.GetGameItemByid(c.productID).iSpr;
            productItem.GetComponentInChildren<TextMeshProUGUI>().text = "x" + c.productAmount.ToString("F0");
        }
        craftingButton.SetActive(selectedRecipeIndex != -1);
    }
    public void CraftButton()
    {
        if (!CheckMaterial()) return;
        foreach (var mat in craftingRecipe.crafts[selectedRecipeIndex].materials)
            player.GetComponent<PlayerData>().RemoveFromInventory(mat.iID, mat.amount);
        player.GetComponent<PlayerData>().AddToInventory(craftingRecipe.crafts[selectedRecipeIndex].productID,
            (int)craftingRecipe.crafts[selectedRecipeIndex].productAmount);
        UpdateCraftingPage();
    }
    private bool CheckMaterial()
    {
        bool valid = true;
        if (selectedRecipeIndex != -1)
        {
            Craft c = recipes[selectedRecipeIndex];
            foreach (var mat in c.materials)
            {
                if (player.GetComponent<PlayerData>().GetItemByID(mat.iID)?.quantity < mat.amount)
                {
                    valid = false;
                    break;
                }
            }
        }
        else valid = false;
        return valid;
    }
    private void OnSelectRecipe(int obj)
    {
        selectedRecipeIndex = obj;
        UpdateCraftingPage();
    }

    private void Update()
    {
        if (player)
        {
            coordinate.text = "X: " + player.transform.position.x.ToString("F1") + " Y: " + player.transform.position.y.ToString("F1");
            healthText.text = player.GetComponent<EntityStats>().currentHealth.ToString() + " <3";
        }
    }

    private void PlayerData_OnInventoryChanged(object sender, EventArgs e)
    {
        UpdateItemSlotsUI();
        UpdatePlayerInventory();
    }
    private void UpdateItemSlotsUI()
    {
        PlayerData playerData = player.GetComponent<PlayerData>();
        var inventory = playerData.GetInventory();

        // Cập nhật lại UI item slots dựa trên dữ liệu inventory hiện tại
        for (int i = 0; i < itemSlotsUI.Length; i++)
        {
            var slot = itemSlotsUI[i];
            var item = inventory[i];

            if (item != null && item.id != string.Empty)
            {
                slot.GetComponent<Image>().sprite = Database.db.GetGameItemByid(item.id).iSpr;
                slot.GetComponentInChildren<TextMeshProUGUI>().text = item.quantity.ToString();
            }
            else
            {
                slot.GetComponent<Image>().sprite = null;
                slot.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
    private void OnDisable()
    {
        if (player)
        {
            PlayerData playerData = player.GetComponent<PlayerData>();
            playerData.OnInventoryChanged -= PlayerData_OnInventoryChanged;
        }
    }
}
