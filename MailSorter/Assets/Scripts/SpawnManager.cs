using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float cooldown = 0f;
    [SerializeField] private GameObject packagePrefab;

    public static Action<int> PackageRemovedEvent;
    private GameObject spawnObject;

    private int packageAmount = 0;

    void Start()
    {
        spawnObject = GameObject.FindGameObjectWithTag("Spawner");
        StartCoroutine(SpawnAfterCooldown());
    }

    void Awake()
    {
        PackageRemovedEvent += UpdateAmount;
    }

    void OnApplicationQuit()
    {
        PackageRemovedEvent -= UpdateAmount;    
    }

    private void UpdateAmount(int delta)
    {
        packageAmount += delta;
    }

    IEnumerator SpawnAfterCooldown()
    {
        while (true)
        {
            if (packageAmount == 0) SpawnPackage();
            else Debug.Log("Уже имеется активная незабранная посылка!");

            yield return new WaitForSeconds(cooldown);
        }
    }

    void SpawnPackage()
    {
        GameObject newpackage = Instantiate(
            packagePrefab, 
            spawnObject.transform.position, 
            spawnObject.transform.rotation
        );

        UpdateAmount(1);
    }
}
