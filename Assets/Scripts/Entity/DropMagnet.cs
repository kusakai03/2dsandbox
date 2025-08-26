using UnityEngine;

public class DropMagnet : MonoBehaviour
{
    public float magnetForce = 20f;
    public float pickUpRange = 5f;
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    [SerializeField] private PlayerInventoryItem item;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < pickUpRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.AddForce(dir * magnetForce);
        }
    }
    public void SetTarget(Transform t)
    {
        player = t;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            player.GetComponent<PlayerData>().AddToInventory(item.id, item.quantity);
            Destroy(gameObject);
        }
    }
}
