using UnityEngine;

public class Package : Interactable
{
    [Header("Данные посылки")]
    [SerializeField] private string destinationCity;

    public string DestinationCity => destinationCity;

    private bool isHeld = false;
    private bool isPlaced = false;

    private bool isHeldBefore = false;

    public bool IsHeld => isHeld;
    public bool IsPlaced => isPlaced;

    public override string GetInteractionPrompt()
    {
        return $"[E] Взять посылку ({destinationCity})";
    }

    public override bool CanInteract(PlayerInteraction player)
    {
        return base.CanInteract(player) && !isHeld && !player.IsHoldingPackage;
    }

    public override void Interact(PlayerInteraction player)
    {
        player.PickupPackage(this);
        if (!isHeldBefore)
        {
            isHeldBefore = true;
            SpawnManager.PackageRemovedEvent?.Invoke(-1);
        }
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
    }

    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
    }

    public void SetDestination(string city)
    {
        destinationCity = city;
    }
}