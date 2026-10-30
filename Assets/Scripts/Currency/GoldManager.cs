using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public int startingGold = 250;
    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        Instance = this;
        Gold = startingGold;
    }

    void Start()
    {
        OnGoldChanged?.Invoke(Gold);
    }

    public bool CanAfford(int cost) => Gold >= cost;

    public void Add(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool TrySpend(int cost)
    {
        if (Gold < cost) return false;
        Gold -= cost;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
}