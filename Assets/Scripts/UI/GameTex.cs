using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTex : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI coordinate;
    [SerializeField] private GameObject[] itemSlotsUI;
    [SerializeField] private TextMeshProUGUI healthText;
    private void Start()
    {
        PlayerData playerData = player.GetComponent<PlayerData>();
        playerData.OnInventoryChanged += PlayerData_OnInventoryChanged;
        UpdateItemSlotsUI();
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
