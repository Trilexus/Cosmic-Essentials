using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpaceShip : SpaceFleet
{
    public Dictionary<ResourceType, ResourceStorage> ResourceStorage = new Dictionary<ResourceType, ResourceStorage>();
    public static int CargoSpace = 500;
    public int FreeSpace;
    public CelestialBody target;
    public CelestialBody origin;
    public bool ReturnToOrigin = false;
    public bool isStarted = false;
    public bool isArrived = false;
    float isArrivedDistance = 0.001f;
    float speed;
    float rotationSpeed = 150f;
    public SpacefleetType Type; // Enum, was für ein Schiff oder Raumstation ist es?
    public int MaxRange; // Wie weit kann es fliegen?
    public int MaxFuel; // Wie viel Treibstoff kann es mitnehmen?
    public int Fuel;
    public int LaunchSpacePointsCosts;
    public SpacefleetScriptableObject SpacefleetScriptableObject;

    [SerializeField]
    public static Dictionary<ResourceType, ResourceStorage> SpaceShipCosts = new Dictionary<ResourceType, ResourceStorage> {
        { ResourceType.Metal, new ResourceStorage(ResourceType.Metal, 100, 0, 0, 0) },
        { ResourceType.Energy, new ResourceStorage(ResourceType.Energy, 100, 0, 0, 0) },
    };

    
    public void ResetResources()
    {
        ResourceStorage.Clear();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceStorage[type] = new ResourceStorage(type.ToString(), 100, 0, 0, 0);
        }
    }

    public void InitializedSpaceShip(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        ResetResources();
        CargoSpace = spacefleetScriptableObject.CargoSpace;        
        speed = spacefleetScriptableObject.Speed / 100f;
        MaxRange = spacefleetScriptableObject.Range;
        MaxFuel = spacefleetScriptableObject.MaxFuel;
        Fuel = spacefleetScriptableObject.Fuel;
        FreeSpace = CargoSpace;
        SpacefleetScriptableObject = spacefleetScriptableObject;
    }



    // Start is called before the first frame update
    void Start()
    {
        RotateToTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.transform.position) < isArrivedDistance)
            {
                isArrived = true;
                isStarted = false;
                target.RegisterTheSpaceship(this);
            }
        }
    }

    public void StartJourney(CelestialBody origin, CelestialBody target, bool fliesBack)
    {
        this.target = target;
        this.origin = origin;
        Fuel -= LaunchSpacePointsCosts;
        SpaceShip spaceShip = this;
        this.transform.position = origin.transform.position;
        RotateToTarget();
        gameObject.SetActive(true);
        isArrived = false;
        isStarted = true;
        ReturnToOrigin = fliesBack;
    }

    public void RefuelSpaceShip(ResourceTransferOrder order)
    {
        int fuelCosts = LaunchSpacePointsCosts;

        if (order.ReturnToOrigin) fuelCosts *= 2;
        order.Origin.GetComponent<CelestialBody>().ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity -= fuelCosts;
        Fuel = fuelCosts;

    }

    private void RotateToTargetOverTime()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Sanfte Rotation zum Ziel
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }
    private void RotateToTarget()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = targetRotation;
    }
}
