using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Action<string> InteractEvent;
    public static Action MoneyUpdatedEvent;

    private EconomyManager economyManager;
    private TextMeshProUGUI interactText;
    private TextMeshProUGUI moneyBalanceText;

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

            interactText = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TextMeshProUGUI>();
            moneyBalanceText = GameObject.FindGameObjectWithTag("MoneyBalanceText").GetComponent<TextMeshProUGUI>();

            uiPanel = GameObject.FindGameObjectWithTag("UIPanel");
        } catch
        {
            return;
        }
    }

    void OnEnable()
    {
        InteractEvent += HandleInteract;
        MoneyUpdatedEvent += HandleUpdateBalance;
        
        GameManager.PauseEvent += HandlePause;
        GameManager.UnpauseEvent += HandleUnpause;
        SceneManager.SceneChangedToMainMenu += InMainMenu;
    }

    void OnDisable()
    {
        InteractEvent -= HandleInteract;
        MoneyUpdatedEvent -= HandleUpdateBalance;
    
        GameManager.PauseEvent -= HandlePause;
        GameManager.UnpauseEvent -= HandleUnpause;

        SceneManager.SceneChangedToMainMenu -= InMainMenu;
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
}
