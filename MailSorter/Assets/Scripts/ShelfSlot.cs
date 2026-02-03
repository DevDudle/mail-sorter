using UnityEngine;

public class ShelfSlot : Interactable
{
    private Package currentPackage;

    public bool IsOccupied => currentPackage != null;
    public Package CurrentPackage => currentPackage;

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
        }
        else if (!player.IsHoldingPackage && IsOccupied)
        {
            Package package = RemovePackage();
            player.PickupPackage(package);
        }
    }

    public void PlacePackage(Package package)
    {
        if (package == null)
        {
            Debug.LogError("PlacePackage: package is null!");
            return;
        }

        currentPackage = package;

        package.transform.SetParent(this.transform);
        package.transform.localPosition = Vector3.zero;
        package.transform.localRotation = Quaternion.identity;

        Rigidbody rb = package.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = package.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        Debug.Log($"Коробка размещена в слоте {gameObject.name}");
    }

    public Package RemovePackage()
    {
        if (currentPackage == null) return null;

        Package package = currentPackage;
        currentPackage = null;

        package.transform.SetParent(null);

        Debug.Log($"Коробка забрана из слота {gameObject.name}");

        return package;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.3f, 0.3f, 0.3f));
    }
}