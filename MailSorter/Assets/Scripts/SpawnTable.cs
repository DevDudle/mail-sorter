using UnityEngine;

public class SpawnTable : Interactable
{
    [Header("Настройки стола")]
    [Tooltip("Точка в воздухе НАД столом")]
    [SerializeField] private Transform packageSpawnPoint;

    public const string SAVE_ID = "SpawnTable_Unique_ID";

    private Package currentPackage;
    public Package CurrentPackage => currentPackage;
    public bool HasPackage => currentPackage != null;

    public override string GetInteractionPrompt()
    {
        return HasPackage ? $"[E] Взять посылку ({currentPackage.DestinationCity})" : "Ожидание посылки...";
    }

    public override bool CanInteract(PlayerInteraction player)
    {
        return base.CanInteract(player) && HasPackage && !player.IsHoldingPackage;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (currentPackage == null) return;

        player.PickupPackage(currentPackage);
        currentPackage = null;

        ShelfSaveManager.Instance?.SaveShelves();
    }

    public void SetPackage(Package package)
    {
        if (package == null) return;
        currentPackage = package;
        package.SetPlaced(true);

        package.transform.SetParent(null);
        package.transform.localScale = Vector3.one;

        if (packageSpawnPoint != null)
        {
            package.transform.position = packageSpawnPoint.position;
            package.transform.rotation = packageSpawnPoint.rotation;
        }

        Rigidbody rb = package.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}