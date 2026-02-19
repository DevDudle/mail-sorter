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

        speedCost = (int)(50 * speedLevel * 1.25f);
        buySpeedText.SetText($"Купить ({speedCost} монет)");

        isBoxCooldownBought = SaveManager.GetSave("BoxCooldownUpgrade", 0) == 1;
        if (isBoxCooldownBought)
        {
            boxCooldownButton.onClick.RemoveAllListeners();
            boxCooldownText.SetText($"Улучшение куплено");
        }

        boxCooldownText.SetText($"Купить (500 монет)");
    }

    public void SpeedUpgrade()
    {
        int currentSpeed = SaveManager.GetSave("Speed", 5);
        PlayerController.SpeedChangedEvent?.Invoke(Mathf.Max(5f, 2.5f * (speedLevel + 1)));
        int newSpeed = SaveManager.GetSave("Speed", 5);

        if (currentSpeed == newSpeed)
        {
            buySpeedText.SetText("Максимальный уровень");
            buySpeedButton.onClick.RemoveAllListeners();

            UIManager.NotificationEvent?.Invoke("error", "Достигнут максимальный уровень скорости!");
            return;
        }

        EconomyManager.AddMoneyWithoutRatioEvent?.Invoke(-speedCost);

        speedLevel += 1;
        SaveManager.SetSave("SpeedLevel", speedLevel);

        speedCost = (int)(20 * speedLevel * 1.25f);

        buySpeedText.SetText($"Купить ({speedCost} монет)");
    }

    public void BuyBoxCooldownShower()
    {
        isBoxCooldownBought = true;
        SaveManager.SetSave("BoxCooldownUpgrade", 1);

        buySpeedText.SetText("Максимальный уровень");
        buySpeedButton.onClick.RemoveAllListeners();
    }
}
