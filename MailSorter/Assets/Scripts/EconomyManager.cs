using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static Action<int> AddMoneyEvent;
    public static Action<int> AddMoneyWithoutRatioEvent;

    private float ratio = 1;
    private int money = 0;

    void Awake()
    {
        UpdateRatio(SaveManager.GetSave("Difficulty", "Normal"));
        money = SaveManager.GetSave("Money", 0);
    }

    void OnEnable()
    {
        AddMoneyEvent += (money) => Add(money, false);
        AddMoneyWithoutRatioEvent += (money) => Add(money, true);


        SettingsManager.DifficultyUpdatedEvent += UpdateRatio;
    }

    void OnDisable()
    {
        AddMoneyEvent -= (money) => Add(money, false);
        AddMoneyWithoutRatioEvent -= (money) => Add(money, true);

        SettingsManager.DifficultyUpdatedEvent -= UpdateRatio;
    }

    void UpdateRatio(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                ratio = 0.5f;
                break;
            case "Normal":
                ratio = 1f;
                break;
            case "Hard":
                ratio = 2f;
                break;
        }

        SaveManager.SetSave("Ratio", ratio);
    }

    private void Add(int delta, bool without_ratio)
    {
        if (without_ratio)
        {
            money += delta;

            UIManager.MoneyUpdatedEvent?.Invoke();
            SaveManager.SetSave("Money", money);
            return;
        }

        if (delta < 0)
        {
            money += (int)(delta * ratio);
        } else
        {
            money += (int)(delta / ratio);
        }

        UIManager.MoneyUpdatedEvent?.Invoke();
        SaveManager.SetSave("Money", money);
    }

    public int Get() => money;
    public float GetRatio() => ratio;
}
