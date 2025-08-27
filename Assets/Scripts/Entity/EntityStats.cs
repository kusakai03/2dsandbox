using System;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    public event EventHandler OnEntityZeroHealth;
    public event EventHandler OnEntityHit;
    [SerializeField] private float maxHeath;
    [SerializeField] private string entityTag;
    public float currentHealth { get; private set; }
    [SerializeField] private float atk;
    private bool d;
    private bool gotHit;
    public float GetFinalATK()
    {
        return atk;
    }
    private void Start()
    {
        currentHealth = maxHeath;
        d = false;
    }
    public void TakeDamage(float damage, Vector3 attackerPos)
    {
        if (!gotHit)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            gotHit = true;
            if (currentHealth <= 0 && !d)
            {
                d = true;
                OnEntityZeroHealth?.Invoke(this, EventArgs.Empty);
                Destroy(gameObject);
                return;
            }

            Invoke("ResetGotHit", 0.5f);
            OnEntityHit?.Invoke(this, EventArgs.Empty);
            KnockbackReceiver knock = GetComponent<KnockbackReceiver>();
            if (knock != null)
            {
                knock.ApplyKnockback(attackerPos);
            }
        }
    }


    private void ResetGotHit()
    {
        gotHit = false;
    }
}
