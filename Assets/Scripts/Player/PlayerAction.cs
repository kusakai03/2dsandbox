using System;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerOpenInventory;
    [SerializeField] private GameObject attackRange;
    [SerializeField] private float attackSpeed;
    private bool canAttack = true;
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
