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
