using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buySpeedText;
    [SerializeField] private Button buySpeedButton;

    [SerializeField] private TextMeshProUGUI boxCooldownText;
    [SerializeField] private Button boxCooldownButton;

    private int speedLevel = 0;
    private int speedCost = 0;

    private bool isBoxCooldownBought = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        speedLevel = SaveManager.GetSave("SpeedLevel", 1);
        if (speedLevel > 1)
        {
            PlayerController.SpeedChangedEvent?.Invoke(5 * speedLevel);
        }

        if (speedLevel * 2.5f + 2.5f != 15f)
        {
            speedCost = (int)(50 * speedLevel * 1.25f);
            buySpeedText.SetText($"Купить ({speedCost} монет)");
        }
        else
        {
            buySpeedButton.onClick.RemoveAllListeners();
            buySpeedText.SetText("Максимальный уровень");
        }

        isBoxCooldownBought = SaveManager.GetSave("BoxCooldownUpgrade", 0) == 1;

        if (isBoxCooldownBought)
        {
            boxCooldownButton.onClick.RemoveAllListeners();
            boxCooldownText.SetText("Улучшение куплено");
        }
        else
        {
            boxCooldownText.SetText("Купить (500 монет)");
            boxCooldownButton.onClick.RemoveAllListeners();
            boxCooldownButton.onClick.AddListener(BuyBoxCooldownShower);
        }
    }

    public void SpeedUpgrade()
    {
        int currentSpeed = SaveManager.GetSave("Speed", 5);

        PlayerController.SpeedChangedEvent?.Invoke(2.5f + 2.5f * (speedLevel + 1));

        if (SaveManager.GetSave("Money", 0) < speedCost)
        {
            UIManager.NotificationEvent?.Invoke("error", "Недостаточно средств!");
            return;
        }

        EconomyManager.AddMoneyWithoutRatioEvent?.Invoke(-speedCost);

        speedLevel += 1;
        SaveManager.SetSave("SpeedLevel", speedLevel);

        speedCost = (int)(50 * speedLevel * 1.25f);
        buySpeedText.SetText($"Купить ({speedCost} монет)");
    }

    public void BuyBoxCooldownShower()
    {
        if (SaveManager.GetSave("Money", 0) < 500)
        {
            UIManager.NotificationEvent?.Invoke("error", "Недостаточно средств!");
            return;
        }

        EconomyManager.AddMoneyWithoutRatioEvent?.Invoke(-500);

        isBoxCooldownBought = true;
        SaveManager.SetSave("BoxCooldownUpgrade", 1);

        boxCooldownText.SetText("Улучшение куплено");
        boxCooldownButton.onClick.RemoveAllListeners();
        boxCooldownButton.interactable = false;
    }
}