using System;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public static Action<Package> PackageInteractEvent;

    void Awake()
    {
        PackageInteractEvent += removePackage;    
    }

    void OnApplicationQuit()
    {
        PackageInteractEvent -= removePackage;    
    }

    public void addPackage(Package package)
    {
        Debug.Log("Package added!");
    }

    public void removePackage(Package package)
    {
        Debug.Log("Package removed!");
    }
}
