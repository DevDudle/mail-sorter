using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;

    public static Action<GameObject> PauseEvent;
    public static Action<GameObject> UnpauseEvent;

    void Start()
    {
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        SpawnManager.PackagesOverflowEvent += FireForOverflow;
    }

    void OnDisable()
    {
        SpawnManager.PackagesOverflowEvent -= FireForOverflow;
    }

    void FireForOverflow()
    {
        float ratio = 1;

        string difficulty = SaveManager.GetSave("Difficulty", "Normal");
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

        EconomyManager.AddMoneyEvent?.Invoke(-20);
        UIManager.NotificationEvent?.Invoke("warning", $"Штраф! Вы выплатили {(int)(20 * ratio)} монет за долгую расфасовку!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (Time.timeScale == 1f)
            {
                Pause();
            }

            else
            {
                Unpause();
            }
        }
    }

    public void Pause() { Time.timeScale = 0f; PauseEvent?.Invoke(PausePanel); }
    public void Unpause() { Time.timeScale = 1f; UnpauseEvent?.Invoke(PausePanel); }
}
