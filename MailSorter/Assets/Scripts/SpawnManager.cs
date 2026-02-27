using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private float cooldown = 30f;           
    [SerializeField] private float minCooldown = 1f;         
    [SerializeField] private float cooldownDecreaseRate = 0.1f; 
    [SerializeField] private GameObject packagePrefab;   
    [SerializeField] private string[] cities = { "Брест", "Кобрин", "Барановичи", "Пинск", "Пружаны" };

    [Header("Ссылки")]
    [SerializeField] private SpawnTable spawnTable;

    public static Action PackagesOverflowEvent;
    public static Action<float> TimerUpdateEvent;

    private const string KEY_COOLDOWN = "Spawn_Cooldown";
    private const string KEY_NEXT_TIME = "Spawn_NextTime";

    void OnDisable()
    {
        PlayerPrefs.SetFloat(KEY_COOLDOWN, cooldown);
        PlayerPrefs.Save();
    }

    void Start()
    {
        if (spawnTable == null)
            spawnTable = FindFirstObjectByType<SpawnTable>();

        if (PlayerPrefs.HasKey(KEY_COOLDOWN))
            cooldown = PlayerPrefs.GetFloat(KEY_COOLDOWN);

        StartCoroutine(StartupRoutine());
    }

    IEnumerator StartupRoutine()
    {
        yield return null;

        float delay = CalculateInitialDelay();

        StartCoroutine(SpawnLoop(delay));
    }

    private float CalculateInitialDelay()
    {
        if (PlayerPrefs.HasKey(KEY_NEXT_TIME))
        {
            long ticks = Convert.ToInt64(PlayerPrefs.GetString(KEY_NEXT_TIME));
            DateTime targetTime = new DateTime(ticks);
            TimeSpan diff = targetTime - DateTime.Now;

            return (float)Math.Max(0, diff.TotalSeconds);
        }

        return 0f;
    }

    IEnumerator SpawnLoop(float initialDelay)
    {
        if (initialDelay > 0)
        {
            yield return StartCoroutine(WaitRoutine(initialDelay));
            TrySpawnPackage();
        }
        else
        {
            if (spawnTable != null && !spawnTable.HasPackage)
            {
                TrySpawnPackage();
            }
        }

        while (true)
        {
            yield return StartCoroutine(WaitRoutine(cooldown));
            TrySpawnPackage();
        }
    }

    IEnumerator WaitRoutine(float duration)
    {
        DateTime target = DateTime.Now.AddSeconds(duration);
        PlayerPrefs.SetString(KEY_NEXT_TIME, target.Ticks.ToString());
        PlayerPrefs.Save();

        float remaining = duration;
        while (remaining > 0)
        {
            TimerUpdateEvent?.Invoke(remaining);
            yield return null;
            remaining -= Time.deltaTime;
        }
        TimerUpdateEvent?.Invoke(0);
    }

    void TrySpawnPackage()
    {
        if (spawnTable == null) return;

        if (!spawnTable.HasPackage)
        {
            SpawnPackage();

            cooldown = Mathf.Max(minCooldown, cooldown - cooldownDecreaseRate);
        }
        else
        {
            PackagesOverflowEvent?.Invoke();
        }
    }

    void SpawnPackage()
    {
        GameObject obj = Instantiate(packagePrefab);
        Package pkg = obj.GetComponent<Package>();

        string randomCity = cities[UnityEngine.Random.Range(0, cities.Length)];
        pkg.SetDestination(randomCity);

        spawnTable.SetPackage(pkg);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(KEY_COOLDOWN);
        PlayerPrefs.DeleteKey(KEY_NEXT_TIME);

        PlayerPrefs.DeleteKey("ShelfData");
        PlayerPrefs.Save();
    }
}