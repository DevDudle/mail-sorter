using UnityEngine;
using System;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Настройки переноски")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Ссылки")]
    [SerializeField] private Camera playerCamera;

    private Package heldPackage;
    public bool IsHoldingPackage => heldPackage != null;
    public Package HeldPackage => heldPackage;

    public static event Action<Package> OnPackagePickedUp;
    public static event Action<Package> OnPackageDropped;

    private Interactable currentInteractable;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = GetComponentInChildren<Camera>();
        }

        if (holdPoint == null)
        {
            GameObject holdObj = new GameObject("HoldPoint_Auto");
            holdObj.transform.SetParent(playerCamera.transform);
            holdObj.transform.localPosition = new Vector3(0.5f, -0.3f, 0.8f);
            holdPoint = holdObj.transform;
        }
    }

    void Update()
    {
        CheckForInteractables();
        HandleInput();
        UpdateHeldPackage();
    }

    void UpdateHeldPackage()
    {
        if (!IsHoldingPackage) return;
        if (heldPackage == null) return;
        if (playerCamera == null) return;

        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        right.Normalize();

        float holdForward = 0.8f;
        float holdRight = 0f;
        float holdHeight = -0.5f;

        Vector3 camPos = playerCamera.transform.position;

        Vector3 targetPos = camPos
            + forward * holdForward
            + right * holdRight
            + Vector3.up * holdHeight;

        heldPackage.transform.position = targetPos;

        float yRotation = playerCamera.transform.eulerAngles.y;
        heldPackage.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void CheckForInteractables()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.green, 0.1f);

        bool hitSomething;
        RaycastHit hit;

        if (interactableLayer != 0)
            hitSomething = Physics.Raycast(ray, out hit, pickupRange, interactableLayer);
        else
            hitSomething = Physics.Raycast(ray, out hit, pickupRange);

        if (hitSomething)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
            }

            if (interactable != null)
            {
                if (interactable.CanInteract(this))
                {
                    currentInteractable = interactable;
                    return;
                }
            }
        }

        currentInteractable = null;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }
    }

    public void PickupPackage(Package package)
    {
        if (IsHoldingPackage)
        {
            return;
        }

        heldPackage = package;

        Rigidbody rb = package.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        package.SetHeld(true);

        OnPackagePickedUp?.Invoke(package);
    }

    public Package ReleasePackage()
    {
        if (!IsHoldingPackage) return null;

        Package package = heldPackage;
        heldPackage = null;

        package.transform.SetParent(null);
        package.SetHeld(false);

        Rigidbody rb = package.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.detectCollisions = true;
            rb.isKinematic = false;
        }

        OnPackageDropped?.Invoke(package);

        return package;
    }
}