using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    public SpacefleetType Type; // Enum, was f�r ein Schiff oder Raumstation ist es?
    public int MaxRange; // Wie weit kann es fliegen?
    public int MaxFuel; // Wie viel Treibstoff kann es mitnehmen?
    public int Fuel;
    public int LaunchSpacePointsCosts;
    public SpacefleetScriptableObject SpacefleetScriptableObject;
    public delegate void OnReachedCelestialBody(GameObject gameObject, SpaceShip spaceShip);
    public static event OnReachedCelestialBody OnReachedCelestialBodyEvent;

    public Flymodes Flymode = Flymodes.FreeFlight;


    [SerializeField]
    public static Dictionary<ResourceType, ResourceStorage> SpaceShipCosts = new Dictionary<ResourceType, ResourceStorage> {
        { ResourceType.Metal, new ResourceStorage(ResourceType.Metal, 100, 0, 0, 0) },
        { ResourceType.Energy, new ResourceStorage(ResourceType.Energy, 100, 0, 0, 0) },
    };

    public void DebugRessources(string message)
    {
        TextMeshProUGUI debugTXT = GUIManager.Instance.DebugText;
        if (ResourceStorage == null)
        {
            debugTXT.text += "NULL:\n";
            return;
        }
        debugTXT.text += message + "#############SpaceShip:\n";        
        foreach (var resource in ResourceStorage)
        {
            debugTXT.text += resource.Key + " - " + resource.Value.StorageQuantity + "\n";
        }
        debugTXT.text += "ScripableObject:\n";
        foreach(var resource in SpacefleetScriptableObject.ResourceStorage)
        {
            debugTXT.text += resource.Key + " - " + resource.Value.StorageQuantity + "\n";
        }
    }

    public void ResetResources()
    {
        ResourceStorage.Clear();        
            ResourceStorage.Add(ResourceType.Food, new ResourceStorage(ResourceType.Food, CargoSpace, 0, 0, 0));
            ResourceStorage.Add(ResourceType.Metal, new ResourceStorage(ResourceType.Metal, CargoSpace, 0, 0, 0));
            ResourceStorage.Add(ResourceType.Energy, new ResourceStorage(ResourceType.Energy, CargoSpace, 0, 0, 0));
            ResourceStorage.Add(ResourceType.SpacePoints, new ResourceStorage(ResourceType.SpacePoints, CargoSpace, 0, 0, 0));
    }

    public void InitializedSpaceShip(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        CargoSpace = spacefleetScriptableObject.CargoSpace;        
        speed = spacefleetScriptableObject.Speed / 100f;
        MaxRange = spacefleetScriptableObject.Range;
        MaxFuel = spacefleetScriptableObject.MaxFuel;
        Fuel = spacefleetScriptableObject.Fuel;
        FreeSpace = CargoSpace;

        Flymode = Flymodes.FreeFlight;
        ResourceStorage = new Dictionary<ResourceType, ResourceStorage>();
        ReturnToOrigin = false;
        isStarted = false;
        isArrived = false;
        isArrivedDistance = 0.001f;
        rotationSpeed = 150f;

        ResetResources();
        SpacefleetScriptableObject = spacefleetScriptableObject;
        DebugRessources("InitializedSpaceShip");
    }



    // Start is called before the first frame update
    void Start()
    {
        RotateToTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted && Flymode == Flymodes.ExecuteOrder)
        {
            OrderFlymode();
        } else if (isStarted && Flymode == Flymodes.FlyToTarget)
        {
        //  Debug.Log("FlyToTarget");
            FlyToTarget();
        }
    }

    public void OrderFlymode()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.transform.position) < isArrivedDistance)
        {
            isArrived = true;
            isStarted = false;
            target.RegisterTheSpaceship(this);
            OnReachedCelestialBodyEvent?.Invoke(target.gameObject, this);
        }
    }

    public void FlyToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.transform.position) < isArrivedDistance)
        {
            Debug.Log("Arrived");
            DebugRessources("Arrived");
            isArrived = true;
            isStarted = false;
            Debug.Log("ArrivedAddSpaceshipToTarget");
            target.PerformHangarOperation(hangar => hangar.AddSpaceShip(this));
            SpaceShipPool.Instance.ReturnSpaceShipToPool(this.gameObject);
            OnReachedCelestialBodyEvent?.Invoke(target.gameObject, this);
        }
    }

    public void StartJourneyOrder(CelestialBody origin, CelestialBody target, bool fliesBack)
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
        Flymode = Flymodes.ExecuteOrder;
    }

    public void StartJourneyFlyToTarget(CelestialBody origin, CelestialBody target)
    {
        
        Debug.Log("StartJourneyFlyToTarget");
        this.target = target;
        this.origin = target;
        Fuel -= LaunchSpacePointsCosts;
        SpaceShip spaceShip = this;
        this.transform.position = origin.transform.position;
        RotateToTarget();
        gameObject.SetActive(true);
        isArrived = false;
        isStarted = true;
        DebugRessources("StartJourneyFlyToTarget");
        ReturnToOrigin = false;
        Flymode = Flymodes.FlyToTarget;
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
public enum Flymodes
{
    FreeFlight,
    ExecuteOrder,
    FlyToTarget
}