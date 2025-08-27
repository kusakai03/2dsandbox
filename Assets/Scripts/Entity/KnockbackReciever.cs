using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    public float knockbackForce = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector3 attackerPos)
    {
        if (rb == null) return;
        Vector2 dir = (transform.position - attackerPos).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        Debug.Log("Knockback applied with force: " + (dir * knockbackForce));
    }
}
