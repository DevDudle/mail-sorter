using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private TMP_InputField VolumeLabel;

    void Start()
    {
        VolumeSlider.onValueChanged.AddListener(VolumeSliderChanged);    
    }

    public void VolumeSliderChanged(float newValue)
    {
        VolumeManager volumeManager = GameObject.FindGameObjectWithTag("VolumeManager").GetComponent<VolumeManager>();
        volumeManager.SetVolume((int)newValue);

        VolumeLabel.text = $"{newValue}%";
    }
}
