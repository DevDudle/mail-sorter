using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Action<string> InteractEvent;
    public static Action MoneyUpdatedEvent;

    private EconomyManager economyManager;
    private TextMeshProUGUI interactText;
    private TextMeshProUGUI moneyBalanceText;

    void Start()
    {
        HandleUpdateBalance();    
    }

    void Awake()
    {
        economyManager = GameObject.FindGameObjectWithTag("EconomyManager").GetComponent<EconomyManager>();

        interactText = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TextMeshProUGUI>();
        moneyBalanceText = GameObject.FindGameObjectWithTag("MoneyBalanceText").GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        InteractEvent += HandleInteract;
        MoneyUpdatedEvent += HandleUpdateBalance;
    }

    void OnDisable()
    {
        InteractEvent -= HandleInteract;
        MoneyUpdatedEvent -= HandleUpdateBalance;
    }

    void HandleUpdateBalance()
    {
        string newBalance = $"Баланс: {economyManager.Get()} руб.";
        moneyBalanceText.SetText(newBalance);
    }

    void HandleInteract(string text)
    {
        if (interactText.text != text && text != null) interactText.SetText(text);
        else interactText.text = "";
    }
}
