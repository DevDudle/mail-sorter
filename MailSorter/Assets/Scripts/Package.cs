using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] private string destinationCity;
    [SerializeField] private float interactionDistance;

    private Backpack playerBackpack;
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        playerBackpack = player.GetComponent<Backpack>();
        playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (distance <= interactionDistance)
            {
                Interact();
            }
        }
    }

    public Package getPackage()
    {
        return this;
    }

    public void setDestination(string city)
    {
        destinationCity = city;
    }

    public void Interact()
    {
        Backpack.PackageInteractEvent.Invoke(this);
        SpawnManager.PackageRemovedEvent.Invoke(-1);

        Destroy(gameObject);
    }
}