using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShelfSaveManager : MonoBehaviour
{
    public static ShelfSaveManager Instance;

    [Header("—сылки")]
    [SerializeField] private List<Package> packagePrefabs;
    [SerializeField] private SpawnTable spawnTable;

    private const string SAVE_KEY = "ShelfData";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (spawnTable == null) spawnTable = FindFirstObjectByType<SpawnTable>();
    }

    private void Start()
    {
        LoadShelves();
    }

    private void OnDisable()
    {
        SaveShelves();
    }

    public void SaveShelves()
    {
        ShelfSaveData saveData = new ShelfSaveData();
        ShelfSlot[] allSlots = FindObjectsOfType<ShelfSlot>();

        foreach (var slot in allSlots)
        {
            if (slot.IsOccupied && slot.CurrentPackage != null)
            {
                AddPackageToSave(saveData, slot.slotID, slot.CurrentPackage);
            }
        }

        if (spawnTable != null && spawnTable.HasPackage)
        {
            AddPackageToSave(saveData, SpawnTable.SAVE_ID, spawnTable.CurrentPackage);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void AddPackageToSave(ShelfSaveData data, string id, Package pkg)
    {
        data.packages.Add(new PackageData
        {
            slotID = id,
            destinationCity = pkg.DestinationCity,
            packageTypeID = pkg.PackageTypeID
        });
    }

    public void LoadShelves()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        ShelfSaveData loadedData = JsonUtility.FromJson<ShelfSaveData>(json);
        if (loadedData == null || loadedData.packages == null) return;

        var slotMap = FindObjectsOfType<ShelfSlot>().ToDictionary(s => s.slotID, s => s);
        var prefabMap = packagePrefabs.ToDictionary(p => p.PackageTypeID, p => p);

        foreach (var data in loadedData.packages)
        {
            Package prefab = null;
            if (!string.IsNullOrEmpty(data.packageTypeID) && prefabMap.ContainsKey(data.packageTypeID))
                prefab = prefabMap[data.packageTypeID];
            else if (packagePrefabs.Count > 0)
                prefab = packagePrefabs[0];

            if (prefab == null) continue;

            Package newPkg = Instantiate(prefab);
            newPkg.SetDestination(data.destinationCity);

            if (data.slotID == SpawnTable.SAVE_ID && spawnTable != null)
            {
                spawnTable.SetPackage(newPkg);
            }
            else if (slotMap.ContainsKey(data.slotID) && !slotMap[data.slotID].IsOccupied)
            {
                slotMap[data.slotID].PlacePackage(newPkg);
            }
            else
            {
                Destroy(newPkg.gameObject);
            }
        }
    }
}

[System.Serializable]
public class ShelfSaveData { public List<PackageData> packages = new List<PackageData>(); }

[System.Serializable]
public class PackageData
{
    public string slotID;
    public string destinationCity;
    public string packageTypeID;
}