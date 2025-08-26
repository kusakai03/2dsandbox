using System;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    public event EventHandler OnEntityZeroHealth;
    [SerializeField] private float maxHeath;
    [SerializeField] private float currentHeath;
    [SerializeField] private float atk;
    private bool d;
    public float GetFinalATK()
    {
        return atk;
    }
    private void Start()
    {
        currentHeath = maxHeath;
        d = false;
    }
    public void TakeDamage(float damage)
    {
        currentHeath = Mathf.Max(0, currentHeath - damage);
        if (currentHeath <= 0 && !d)
        {
            d = true;
            OnEntityZeroHealth?.Invoke(this, EventArgs.Empty);
        }
    }
}
