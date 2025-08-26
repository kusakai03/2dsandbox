using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private GameObject attackRange;
    public void Attack()
    {
        GetComponent<PlayerMovement>().canMove = false;
        attackRange.SetActive(true);
        Invoke(nameof(DoneAttack), 0.2f);
    }
    private void DoneAttack()
    {
        GetComponent<PlayerMovement>().canMove = true;
        attackRange.SetActive(false);
    }
    private void OnDisable()
    {
        CancelInvoke(nameof(DoneAttack));
    }
}
