using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer mainMixer;

    private float volume;

    private int percentageVolume;
    private float dbVolume;

    void Start()
    {
        percentageVolume = SaveManager.GetSave("PercentageVolume", 0);
        SetVolume(percentageVolume);
    }

    float toDecibel(float percentageVolume)
    {
        dbVolume = 20 * Mathf.Log10(percentageVolume / 100f);
        if (percentageVolume == 0) dbVolume = -80;
        
        return dbVolume;
    }

    public void SetVolume(int newVolume)
    {
        volume = toDecibel(newVolume);
        SaveManager.SetSave("PercentageVolume", newVolume);

        mainMixer.SetFloat("MasterVolume", volume);
    }
}
