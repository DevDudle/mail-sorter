using System.Collections;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingProgressText;

    public void ChangeScene(int sceneNumber)
    {
        StartCoroutine(Handle(sceneNumber));
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator Handle(int sceneNumber)
    {
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
