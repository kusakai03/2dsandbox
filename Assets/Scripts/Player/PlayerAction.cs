using System;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerOpenInventory;
    [SerializeField] private GameObject attackRange;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float defaultAttackSpeed;
    public string weaponType { get; private set; }
    private bool canAttack = true;
    private void Start()
    {
        GetComponent<PlayerData>().OnInventoryChanged += PlayerAction_OnInventoryChanged;
    }

    private void PlayerAction_OnInventoryChanged(object sender, EventArgs e)
    {
        PlayerData playerData = GetComponent<PlayerData>();
        EntityStats stats = GetComponent<EntityStats>();
        if (Database.db.GetGameItemByid(playerData.GetCurrentInvIndex().id)?.type == "weapon")
        {
            attackSpeed = Database.db.GetGameItemByid(playerData.GetCurrentInvIndex().id).weaponData.wAttackSpeed;
            stats.SetATKBonus(Database.db.GetGameItemByid(playerData.GetCurrentInvIndex().id).weaponData.wATK);
            weaponType = Database.db.GetGameItemByid(playerData.GetCurrentInvIndex().id).weaponData.wType;
        }
        else
        {
            attackSpeed = defaultAttackSpeed;
            weaponType = "hand";
            stats.SetATKBonus(0);
        }
    }

    public void Attack()
    {
        if (!canAttack) return;

        canAttack = false;
        GetComponent<PlayerMovement>().canMove = false;
        attackRange.SetActive(true);
        Invoke(nameof(DoneAttack), 0.2f);
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
    private void DoneAttack()
    {
        GetComponent<PlayerMovement>().canMove = true;
        attackRange.SetActive(false);
        Invoke(nameof(ResetAttack), attackSpeed);
    }
    private void OnDisable()
    {
        CancelInvoke(nameof(DoneAttack));
    }
    private void ResetAttack()
    {
        canAttack = true;
    }
    public void OpenInventory()
    {
        OnPlayerOpenInventory?.Invoke(this, EventArgs.Empty);
    }
}
