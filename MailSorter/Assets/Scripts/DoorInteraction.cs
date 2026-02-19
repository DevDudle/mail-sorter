using UnityEngine;

public class DoorInteraction : Interactable
{
    [SerializeField] private SceneManager sceneManager;
    [SerializeField] private EconomyManager economyManager;

    public override void Interact(PlayerInteraction player)
    {
        SaveManager.SetSave("Money", economyManager.Get());
        sceneManager.ChangeScene(2);
    }
}
