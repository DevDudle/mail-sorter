using UnityEngine;

public interface IInteractable
{
    string GetInteractionPrompt();
    bool CanInteract(PlayerInteraction player);
    void Interact(PlayerInteraction player);
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactionPrompt = "ֽאזלטעו E";
    [SerializeField] protected float interactionDistance = 2f;

    public virtual string GetInteractionPrompt() => interactionPrompt;

    public virtual bool CanInteract(PlayerInteraction player)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= interactionDistance;
    }

    public abstract void Interact(PlayerInteraction player);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}