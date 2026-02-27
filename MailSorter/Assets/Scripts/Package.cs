using UnityEngine;

public class Package : Interactable
{
    [Header("Идентификация")]
    [Tooltip("ID типа коробки для сохранения. Должен совпадать в префабе.")]
    public string PackageTypeID = "StandardBox";

    [Header("Данные")]
    [SerializeField] private string destinationCity;

    public string DestinationCity => destinationCity;

    private bool isHeld = false;
    private bool isPlaced = false;

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
    }

    public void SetHeld(bool held) => isHeld = held;
    public void SetPlaced(bool placed) => isPlaced = placed;

    public void SetDestination(string city)
    {
        destinationCity = city;
    }
}