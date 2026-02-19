using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private TMP_InputField VolumeLabel;

    [SerializeField] private TMP_Dropdown DifficultyDropdown;

    public static Action<string> DifficultyUpdatedEvent;

    void Start()
    {
        VolumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
        VolumeSlider.value = SaveManager.GetSave("PercentageVolume", 0);
        VolumeLabel.text = $"{VolumeSlider.value}%";

        DifficultyDropdown.onValueChanged.AddListener(DifficultyChanged);

        string difficulty = SaveManager.GetSave("Difficulty", "Normal");
        int difficultyIndex = 1;

        switch (difficulty)
        {
            case "Easy":
                difficultyIndex = 0; 
                break;

            case "Normal":
                difficultyIndex = 1;
                break;

            case "Hard":
                difficultyIndex = 2;
                break;
        }

        DifficultyDropdown.value = difficultyIndex;
    }

    public void VolumeSliderChanged(float newValue)
    {
        VolumeManager volumeManager = GameObject.FindGameObjectWithTag("VolumeManager").GetComponent<VolumeManager>();
        volumeManager.SetVolume((int)newValue);

        VolumeLabel.text = $"{newValue}%";
    }

    public void DifficultyChanged(int newDifficulty)
    {
        string difficultyCode = "Normal";
        switch (newDifficulty)
        {
            case 0:
                difficultyCode = "Easy";
                break;

            case 1:
                difficultyCode = "Normal";
                break;

            case 2:
                difficultyCode = "Hard";
                break;
        }

        SaveManager.SetSave("Difficulty", difficultyCode);
        DifficultyUpdatedEvent?.Invoke(difficultyCode);
    }
}
