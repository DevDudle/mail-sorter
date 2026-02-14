using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private float cooldown = 3f;
    [SerializeField] private float minCooldown = 1f;
    [SerializeField] private float cooldownDecreaseRate = 0.1f;
    [SerializeField] private GameObject packagePrefab;

    [Header("Города назначения")]
    [SerializeField] private string[] cities = { "Брест", "Кобрин", "Барановичи", "Пинск", "Пружаны" };

    [Header("Ссылки")]
    [SerializeField] private SpawnTable spawnTable;

    public static Action<int> PackageRemovedEvent;

    private int activePackages = 0;

    void Awake()
    {
        PackageRemovedEvent += UpdateAmount;
    }

    void OnDestroy()
    {
        PackageRemovedEvent -= UpdateAmount;
    }

    void Start()
    {
        if (spawnTable == null)
        {
            spawnTable = FindFirstObjectByType<SpawnTable>();
        }

        StartCoroutine(SpawnRoutine());
    }

    void UpdateAmount(int delta)
    {
        activePackages += delta;
        activePackages = Mathf.Max(0, activePackages);
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);

            if (activePackages == 0)
            {
                SpawnPackage();

                cooldown = Mathf.Max(minCooldown, cooldown - cooldownDecreaseRate);
            }
        }
    }

    void SpawnPackage()
    {
        GameObject packageObj = Instantiate(packagePrefab);
        Package package = packageObj.GetComponent<Package>();

        string randomCity = cities[UnityEngine.Random.Range(0, cities.Length)];
        package.SetDestination(randomCity);

        spawnTable.SetPackage(package);
        UpdateAmount(1);
    }
}