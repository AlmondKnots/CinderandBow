using System;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance { get; private set; }

    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    public event Action<int, int> OnHealthChanged;  
    public event Action OnBaseDestroyed;

    void Awake()
    {
        Instance = this;
        CurrentHealth = maxHealth;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth == 0)
            OnBaseDestroyed?.Invoke();
    }
}