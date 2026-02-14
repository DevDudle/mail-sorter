using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public Action<int> AddMoneyEvent;
    private int money = 0;

    void Awake()
    {
        money = SaveManager.GetSave("Money", 0);
    }

    void OnEnable()
    {
        AddMoneyEvent += Add;
    }

    void OnDisable()
    {
        AddMoneyEvent -= Add;
    }

    private void Add(int delta)
    {
        money += delta;
        UIManager.MoneyUpdatedEvent?.Invoke();
    }

    public int Get() => money;
}
