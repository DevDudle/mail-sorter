using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Action<string> InteractEvent;
    public static Action MoneyUpdatedEvent;
    public static Action<float> TimerUpdatedEvent;
    public static Action<string, string> NotificationEvent;

    private EconomyManager economyManager;
    private TextMeshProUGUI interactText;
    private TextMeshProUGUI moneyBalanceText;
    private TextMeshProUGUI notificationText;
    private TextMeshProUGUI cooldownText;
    private TextMeshProUGUI inHandText;

    private GameObject uiPanel;

    void Start()
    {
        HandleUpdateBalance();    
    }

    void Awake()
    {
        try
        {
        economyManager = GameObject.FindGameObjectWithTag("EconomyManager").GetComponent<EconomyManager>();
        }
        catch { }

        try
        {
            interactText = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TextMeshProUGUI>();
        }
        catch { }

        try
        {
            moneyBalanceText = GameObject.FindGameObjectWithTag("MoneyBalanceText").GetComponent<TextMeshProUGUI>();
        }
        catch { }

        try
        {
            notificationText = GameObject.FindGameObjectWithTag("NotificationText").GetComponent<TextMeshProUGUI>();
        }
        catch { }

        try
        {
            inHandText = GameObject.FindGameObjectWithTag("InHandText").GetComponent<TextMeshProUGUI>();
        }
        catch { }

        try
        {
            uiPanel = GameObject.FindGameObjectWithTag("UIPanel");
        }
        catch { }

        try
        {
            cooldownText = GameObject.FindGameObjectWithTag("CooldownText").GetComponent<TextMeshProUGUI>();
        }
        catch { }
    }

    void OnEnable()
    {
        InteractEvent += HandleInteract;
        MoneyUpdatedEvent += HandleUpdateBalance;
        TimerUpdatedEvent += HandleUpdateTimer;
        NotificationEvent += HandleNotifications;
        
        GameManager.PauseEvent += HandlePause;
        GameManager.UnpauseEvent += HandleUnpause;
        SceneManager.SceneChangedToMainMenu += InMainMenu;
        
        PlayerInteraction.OnPackagePickedUp += HandleInHandText;
        PlayerInteraction.OnPackageDropped += HandleInHandText;
    }

    void OnDisable()
    {
        InteractEvent -= HandleInteract;
        MoneyUpdatedEvent -= HandleUpdateBalance;
        TimerUpdatedEvent -= HandleUpdateTimer;
        NotificationEvent -= HandleNotifications;

        GameManager.PauseEvent -= HandlePause;
        GameManager.UnpauseEvent -= HandleUnpause;
        SceneManager.SceneChangedToMainMenu -= InMainMenu;

        PlayerInteraction.OnPackagePickedUp -= HandleInHandText;
        PlayerInteraction.OnPackageDropped -= HandleInHandText;
    }

    IEnumerator ShowCursorRoutine()
    {
        yield return new WaitForEndOfFrame();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void InMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(ShowCursorRoutine());
    }

    private void HandleUpdateTimer(float current)
    {
        if (SaveManager.GetSave("BoxCooldownUpgrade", 0) == 1)
        {
            cooldownText.SetText($"Осталось {Math.Round(current, 2)} сек.\n до появления коробки");
            return;
        }

        cooldownText.SetText("");
    }

    private void HandleUpdateBalance()
    {
        if (moneyBalanceText == null) return;

        string newBalance = $"{economyManager.Get()}";
        moneyBalanceText.SetText(newBalance);
    }

    private void HandleInteract(string text)
    {
        if (text != null) interactText.SetText(text);
        else interactText.text = "";
    }

    private void HandlePause(GameObject pausePanel)
    {
        pausePanel.SetActive(true);
        uiPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleUnpause(GameObject pausePanel)
    {
        pausePanel.SetActive(false);
        uiPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator ShowNotification(string text, Color color)
    {
        notificationText.color = color;
        notificationText.SetText(text);

        yield return new WaitForSeconds(2);

        notificationText.SetText("");
    }

    private void HandleNotifications(string type, string text)
    {
        if (type == "default")
        {
            StartCoroutine(ShowNotification(text, Color.white));
        } 
        else
        {
            StartCoroutine(ShowNotification(text, Color.red));
        }
    }

    private void HandleInHandText(Package package)
    {
        string city = package.DestinationCity;

        if (city == null || !package.IsHeld)
        {
            inHandText.SetText("В руках: Ничего");
            return;
        }

        inHandText.SetText($"В руках: Посылка в {city}");
    }
}
