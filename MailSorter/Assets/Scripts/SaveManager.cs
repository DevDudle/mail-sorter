using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private void Start()
    {
        SetSave("Money", 0);
        SetSave("SpeedLevel", 1);
        SetSave("BoxCooldownUpgrade", 0);
    }

    public static string GetSave(string variable, string defaultValue = "") => PlayerPrefs.GetString(variable, defaultValue);
    public static int GetSave(string variable, int defaultValue = 0) => PlayerPrefs.GetInt(variable, defaultValue);
    public static float GetSave(string variable, float defaultValue = 0) => PlayerPrefs.GetFloat(variable, defaultValue);

    public static void SetSave(string variable, string value) => PlayerPrefs.SetString(variable, value);
    public static void SetSave(string variable, int value) => PlayerPrefs.SetInt(variable, value);
    public static void SetSave(string variable, float value) => PlayerPrefs.SetFloat(variable, value);
}
