using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static Action<TextMeshProUGUI> ResetEvent;

    void OnEnable()
    {
        ResetEvent += (tmp) => { StartCoroutine(ResetHandle(tmp)); };
    }

    void OnDisable()
    {
        ResetEvent -= (tmp) => { StartCoroutine(ResetHandle(tmp)); };
    }

    public static string GetSave(string variable, string defaultValue = "") => PlayerPrefs.GetString(variable, defaultValue);
    public static int GetSave(string variable, int defaultValue = 0) => PlayerPrefs.GetInt(variable, defaultValue);
    public static float GetSave(string variable, float defaultValue = 0) => PlayerPrefs.GetFloat(variable, defaultValue);

    public static void SetSave(string variable, string value) { PlayerPrefs.SetString(variable, value); PlayerPrefs.Save(); }
    public static void SetSave(string variable, int value) { PlayerPrefs.SetInt(variable, value); PlayerPrefs.Save(); }
    public static void SetSave(string variable, float value) { PlayerPrefs.SetFloat(variable, value); PlayerPrefs.Save(); }

    public static IEnumerator ResetHandle(TextMeshProUGUI resetButtonText)
    {
        SpawnManager.ResetData();
        SetSave("PercentageVolume", 50);
        SetSave("BoxCooldownUpgrade", 0);
        SetSave("Difficulty", "Normal");
        SetSave("SpeedLevel", 1);
        SetSave("ShelfData", "");
        SetSave("MoveSpeed", 5);
        SetSave("Cooldown", 30);
        SetSave("Money", 0);

        PlayerPrefs.Save();

        resetButtonText.SetText("Прогресс успешно сброшен!");
        SceneManager.ChangeSceneEvent?.Invoke(0);

        yield return new WaitForSeconds(1);
        resetButtonText.SetText("Сбросить прогресс");

        yield return null;
    }
}
