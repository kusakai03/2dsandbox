using System.Collections.Generic;
using UnityEngine;

public class AttackRangeDetect : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private EntityStats master;
    private List<EntityStats> targets = new();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Entity"))
        {
            targets.Add(collision.GetComponent<EntityStats>());
        }
    }
    private void OnEnable()
    {
        targets.Clear();
        transform.localPosition = Vector2.zero;
    }
    private void OnDisable()
    {
        foreach (var target in targets)
        {
            float damage = master.GetFinalATK();
            if (target.entityTag == "Tree" && master.GetComponent<PlayerAction>().weaponType == "axe")
            {
                damage *= 2;
            }
            target.TakeDamage(damage, master.transform.position);
        }
        if (targets.Count > 0)
        {
            PlayerData pd = master.GetComponent<PlayerData>();
            if (pd != null && pd.GetCurrentInvIndex().id != string.Empty &&
            Database.db.GetGameItemByid(pd.GetCurrentInvIndex().id).type == "weapon")
                pd.DrainDurability(pd.currentIndex, 1);
        }
    }
    private void Update()
    {
        if (movement.dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.dir.y, movement.dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
