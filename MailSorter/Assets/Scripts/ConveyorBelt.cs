using UnityEngine;
using System;
using TMPro;

public class ConveyorBelt : Interactable
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private string destinationCity;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Transform beltSurface;
    [SerializeField] private Transform finishPoint;
    [SerializeField] private float finishRadius = 0.5f;
    [SerializeField] private Transform placePoint;

    [SerializeField] private EconomyManager economyManager;

    public static event Action<Package, string> PackageSentEvent;

    void Start()
    {
        text.SetText(destinationCity);
    }

    void FixedUpdate()
    {
        MovePackagesOnBelt();
        CheckPackageArrival();
    }

    private void MovePackagesOnBelt()
    {
        if (beltSurface == null || finishPoint == null) return;

        Vector3 center = beltSurface.position + (beltSurface.up * 0.2f);
        Vector3 size = beltSurface.GetComponent<Collider>().bounds.size;
        size.y = 0.2f;

        Collider[] hits = Physics.OverlapBox(center, size / 2, beltSurface.rotation);

        foreach (Collider hit in hits)
        {
            Package pkg = hit.GetComponent<Package>();
            if (pkg == null || !pkg.gameObject.activeInHierarchy) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                Vector3 direction = (finishPoint.position - rb.position).normalized;
                direction.y = 0;

                Vector3 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }
        }
    }

    private void CheckPackageArrival()
    {
        if (finishPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(finishPoint.position, finishRadius);

        foreach (Collider hit in hits)
        {
            Package pkg = hit.GetComponent<Package>();

            if (pkg != null && pkg.gameObject.activeInHierarchy)
            {
                pkg.gameObject.SetActive(false);

                HandleSent(pkg, destinationCity);

                Destroy(pkg.gameObject);
            }
        }
    }

    public override string GetInteractionPrompt()
    {
        return "Положить посылку";
    }

    public override bool CanInteract(PlayerInteraction player)
    {
        return base.CanInteract(player) && player.IsHoldingPackage;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.IsHoldingPackage)
        {
            Package package = player.ReleasePackage();

            if (package != null)
            {
                if (placePoint != null)
                {
                    package.transform.position = placePoint.position;
                    package.transform.rotation = placePoint.rotation;
                }
                else
                {
                    package.transform.position = beltSurface.position + Vector3.up * 0.5f;
                }

                Rigidbody rb = package.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }
        }
    }

    private void HandleSent(Package package, string destinationCity)
    {
        string packageDestination = package.DestinationCity;
        if (packageDestination != destinationCity)
        {
            economyManager.AddMoneyEvent?.Invoke(-20);
        } 
        else
        {
            economyManager.AddMoneyEvent?.Invoke(30);
        }
    }
}