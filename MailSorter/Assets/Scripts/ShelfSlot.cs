using UnityEngine;

public class ShelfSlot : Interactable
{
    [Header("Настройки слота")]
    [Tooltip("Уникальный ID слота. Генерируется автоматически, если пусто.")]
    public string slotID;

    private Package currentPackage;

    public bool IsOccupied => currentPackage != null;
    public Package CurrentPackage => currentPackage;

    private void Awake()
    {
        GenerateID();
    }

    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        if (string.IsNullOrEmpty(slotID))
        {
            string parentName = transform.parent != null ? transform.parent.name : "NoParent";
            string grandParentName = (transform.parent != null && transform.parent.parent != null) ? transform.parent.parent.name : "NoGrandParent";
            slotID = $"{grandParentName}_{parentName}_{transform.name}";
        }
    }

    public override string GetInteractionPrompt()
    {
        if (IsOccupied)
        {
            return $"[E] Взять посылку ({currentPackage.DestinationCity})";
        }
        else
        {
            return "[E] Положить посылку";
        }
    }

    public override bool CanInteract(PlayerInteraction player)
    {
        bool canPlace = player.IsHoldingPackage && !IsOccupied;
        bool canTake = !player.IsHoldingPackage && IsOccupied;

        return base.CanInteract(player) && (canPlace || canTake);
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.IsHoldingPackage && !IsOccupied)
        {
            Package package = player.ReleasePackage();
            PlacePackage(package);

            if (ShelfSaveManager.Instance != null)
                ShelfSaveManager.Instance.SaveShelves();
        }
        else if (!player.IsHoldingPackage && IsOccupied)
        {
            Package package = RemovePackage();
            player.PickupPackage(package);

            if (ShelfSaveManager.Instance != null)
                ShelfSaveManager.Instance.SaveShelves();
        }
    }

    public void PlacePackage(Package package)
    {
        if (package == null) return;

        currentPackage = package;
        package.SetPlaced(true);

        package.transform.SetParent(this.transform);
        package.transform.localPosition = Vector3.zero;
        package.transform.localRotation = Quaternion.identity;

        Rigidbody rb = package.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = package.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    public Package RemovePackage()
    {
        if (currentPackage == null) return null;

        Package package = currentPackage;
        currentPackage = null;
        package.SetPlaced(false);

        package.transform.SetParent(null);

        return package;
    }
}