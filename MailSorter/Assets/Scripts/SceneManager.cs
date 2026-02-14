using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingProgressText;

    public static Action SceneChangedToMainMenu;

    public void ChangeScene(int sceneNumber)
    {
        if (sceneNumber == 0)
        {
            SceneChangedToMainMenu?.Invoke();
        }

        StartCoroutine(Handle(sceneNumber));
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Save()
    {
        SaveManager saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        EconomyManager economyManager = GameObject.FindGameObjectWithTag("EconomyManager").GetComponent<EconomyManager>();

        SaveManager.SetSave("Money", economyManager.Get());
    }

    public void SaveAndExit()
    {
        Save();
        Exit();
    }

    IEnumerator Handle(int sceneNumber)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1) {
            Save();
        }

        loadingPanel.SetActive(true);

        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNumber);
        while (!loading.isDone)
        {
            string progressText = $"Загрузка. . . {Mathf.RoundToInt(loading.progress * 100)}%";
            loadingProgressText.SetText(progressText);
            yield return null;
        }

        loadingPanel.SetActive(false);
    }
}
