using UnityEngine;

public class SpawnTable : Interactable
{
    [Header("Настройки стола")]
    [SerializeField] private Transform packageSpawnPoint;

    private Package currentPackage;

    public bool HasPackage => currentPackage != null;

    public override string GetInteractionPrompt()
    {
        if (currentPackage != null)
        {
            return $"[E] Взять посылку ({currentPackage.DestinationCity})";
        }
        return "Посылок нет";
    }

    public override bool CanInteract(PlayerInteraction player)
    {
        return base.CanInteract(player) &&
               HasPackage &&
               !player.IsHoldingPackage;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (currentPackage == null) return;

        player.PickupPackage(currentPackage);
        currentPackage = null;

        SpawnManager.PackageRemovedEvent?.Invoke(-1);
    }

    public void SetPackage(Package package)
    {
        currentPackage = package;
        package.transform.position = packageSpawnPoint.position;
        package.transform.rotation = packageSpawnPoint.rotation;
    }
}