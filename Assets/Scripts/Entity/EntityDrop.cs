using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityDrop : MonoBehaviour
{
    [SerializeField] private EntityStats stats;
    [SerializeField] private List<ItemDrop> drops;
    private void Start()
    {
        stats.OnEntityZeroHealth += OnEntityZeroHealth;
    }
    private void OnDisable()
    {
        stats.OnEntityZeroHealth -= OnEntityZeroHealth;
    }

    private void OnEntityZeroHealth(object sender, EventArgs e)
    {
        foreach (var d in drops)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= d.dropChance)
            {
                for (int i = 1; i <= d.dropAmount; i++)
                {
                    DropItem(d.itemDrop, transform.position);
                }
            }
        }
    }
    public float dropForce = 5f;
    public void DropItem(GameObject dropItem, Vector2 position)
    {
        GameObject item = Instantiate(dropItem, position, Quaternion.identity);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
        }
    }
}
[Serializable]
public class ItemDrop
{
    public GameObject itemDrop;
    [Range(0f, 1f)]
    public float dropChance;
    public int dropAmount;
}