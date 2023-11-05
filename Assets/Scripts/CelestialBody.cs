using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

abstract public class CelestialBody : MonoBehaviour
{
    [SerializeField]
    protected int maxAreas;
    [SerializeField]
    protected Population population;
    [SerializeField]
    public float interval = 2f; // Zeit zwischen zwei Ticks, wird durch ProductivityRate beeinflusst
    // Every Celestial Body have a default production of ressources.
    [SerializeField]
    private List<Resource> BaseResourceProduction;
    public SpriteRenderer ChildSpriteRenderer;
    public float ProductivityRateBasicValue = 1f;
    public float ProductivityRate = 1f; 
    //Minimalwerte für die ProductivityRate
    public float MinProductivityRate = 0.1f;
    //Maximalwerte für die ProductivityRate
    public float MaxProductivityRate = 5f;
    private float nextTickTime; // Zeitpunkt des nächsten Ticks
    private float timeUntilNextTick; // Verbleibende Zeit bis zum nächsten Tick
    public TextMeshProUGUI TimeToTick;
    public float ConstructionRate = 0.2f; //bigger = faster
    [SerializeField]
    public List<Area> Areas = new List<Area>();
    public List<StructureScriptableObject> StartStructureScriptableObjects;
    public AllowedLocation AllowedLocationType;
    //public List<ResourceStorage> ResourceStorageCelestialBody = new List<ResourceStorage>();
    public StringBuilder sb = new StringBuilder();
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody = new Dictionary<ResourceType, ResourceStorage>();
    public List<ResourceTransferOrder> ResourceTransferOrders = new List<ResourceTransferOrder>();
    public int SpaceShipTransporterAvailable = 0;

    public List <SpaceShip> SpacecraftReadyForUnloading = new List<SpaceShip>();
    public const int OneHundredPercent = 100;

    ResourceTransferDispatcher orderDispatcher;
    public int MaxOrderLoops = 5;
    

    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoTop;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoLeft;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoRight;

    public virtual void Start()
    {
        interval = Random.Range(1.5f, 2.5f);
        interval = (float)Math.Round(interval, 2);
        InitializeResources();
        nextTickTime = Time.time + interval;
        StartCoroutine(TickCoroutine());
        InitializeStartingBuildings();
        orderDispatcher = new ResourceTransferDispatcher(ResourceStorageCelestialBody);
    }
    public void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceStorageCelestialBody[type] = new ResourceStorage(type.ToString(), 1000, 0, 0, 0);
        }
    }

    private void InitializeStartingBuildings()
    {
        foreach (var structure in StartStructureScriptableObjects)
        {
            Structure startbuilding = new Structure(structure);
            Areas.Add(new Area { structure = startbuilding, constructionProgress = 100 });
        }
    }

    protected virtual void Update()
    {
        // Aktualisiere die verbleibende Zeit
        timeUntilNextTick = nextTickTime - Time.time;
        // Aktualisiere die Anzeige
        TimeToTick.SetText(timeUntilNextTick.ToString("0.0") + "/" + interval);
    }

    private IEnumerator TickCoroutine()
    {
        while (true)
        {
            nextTickTime = Time.time + interval; // Setze den Zeitpunkt des nächsten Ticks           
            yield return new WaitForSeconds(interval);
            Tick();
        }
    }

    protected virtual void Tick()
    {
        ExecuteConstructionProgress();
        ManageResourceProduction();
        UnloadTheSpaceShips();
        ProcessResourceTransferOrders();
        GUIManager.Instance.UpdateCelestialBodyInfos();
    }


    public void OnMouseDown()
    {
        Debug.Log("Clicked: " + gameObject.name);
        //GetComponentInParent<PlanetarySystem>().IsActivePlanetarySystem = true;
        GUIManager.Instance.SetActiveCelestialBody(gameObject);
    }
    public void InitiateConstructionStructure(Structure structure)
    {
        //Start building projects (farm, power plant, mine, research center).
        if (maxAreas > Areas.Count)
        {
            Areas.Add(new Area { structure = structure, constructionProgress = 0 });
        }

    }
    public void InitiateDemolishStructure(Structure structure)
    {

        //demolish structure instantly
        var areaToRemove = Areas.LastOrDefault(x => x.structure.Type == structure.Type);
        Debug.Log(structure.Name);
        Debug.Log("Demolish Structure: " + areaToRemove.structure.Name);
        if (areaToRemove != null)
        {
            Areas.Remove(areaToRemove);
        }

    }

    private void ExecuteConstructionProgress()
    {
        int countOfConstructingAreas = Areas.Count(x => x.constructionProgress < 100);
        if (countOfConstructingAreas > 0)
        {
            float individualConstructionRate = ConstructionRate / countOfConstructingAreas;
            Areas.Where(x => x.constructionProgress < 100)
                 .ToList()
                 .ForEach(x => x.constructionProgress += (int)(individualConstructionRate * 100 * ProductivityRate));//TODO: Magic Number
            Areas.Where(x => x.constructionProgress < 100).ToList().ForEach(x => x.constructionProgress = Mathf.Clamp(x.constructionProgress, 1, 100));
        }
        //Executes building projects over time (farm, power plant, mine, research center).
    }



    public void RegisterTheSpaceship(SpaceShip spaceShip)
    {
        SpacecraftReadyForUnloading.Add(spaceShip);
    }

    public virtual void UnloadTheSpaceShips()
    {
        //Unload the SpaceShips
        foreach (SpaceShip spaceship in SpacecraftReadyForUnloading)
        {
            foreach (ResourceStorage resource in spaceship.ResourceStorageSpaceShip.Values)
            {
                ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity += resource.StorageQuantity;
                resource.StorageQuantity = 0; //SpaceShip ist nun leer.
            }
            if (spaceship.ReturnToOrigin)
            {
                spaceship.StartJourney(this, spaceship.origin, false);
            }else
            {
                SpaceShip_TransporterMisslePool.Instance.ReturnSpaceShipToPool(spaceship.gameObject);
                SpaceShipTransporterAvailable += 1; //SpaceShip ist nun auf diesem CelestialBody verfügbar.
            }
        }
        SpacecraftReadyForUnloading.Clear();
    }


    public void ManageResourceProduction()
    {
        // Reset the production of resources to 0.
        foreach (var resourceStorage in ResourceStorageCelestialBody.Values)
        {
            resourceStorage.ResetResourceProduction();
        }
        // Add the base production of resources to the resource storage.
        foreach (Resource resource in BaseResourceProduction)
        {
            if (ResourceStorageCelestialBody.TryGetValue(resource.ResourceType, out var resourceStorage))
            {
                resourceStorage.ProductionQuantity += resource.Quantity;
            }
        }
        // Combine buildings and other factors and calculate the production of resources.
        foreach (var area in Areas)
        {
            if (area.constructionProgress >= 100)
            {
                foreach (var resource in area.structure.Resources)
                {
                    if (ResourceStorageCelestialBody.TryGetValue(resource.ResourceType, out var resourceStorage))
                    {
                        var production = resource.Quantity > 0 ? (int)(resource.Quantity * ProductivityRate) : resource.Quantity;
                        resourceStorage.ProductionQuantity += production > 0 ? production : 0;
                        resourceStorage.ConsumptionQuantity += production < 0 ? production : 0;
                    }
                }
            }
        }
        //Innerhalb der Storage Klassse wird die ProductionQuantity zur StorageQuantity addiert. Das darf nicht mehrfach ausgeführt werden.
        // Update storage.
        foreach (var resourceStorage in ResourceStorageCelestialBody.Values)
        {
            resourceStorage.CalculateResourceStorage();
        }

        // Population consumes food 
        if (ResourceStorageCelestialBody.TryGetValue(ResourceType.Food, out var foodStorage))
        {
            foodStorage.StorageQuantity -= population.CurrentPopulation;
            foodStorage.ConsumptionQuantity -= population.CurrentPopulation;
        }
    }

    private void ProcessResourceTransferOrders()
    {
        bool OrderAvailable = ResourceTransferOrders.Count > 0;
        bool SpaceShipAvailable = SpaceShipTransporterAvailable > 0;
        bool SpacePointsAvailableForStart = ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity > SpaceShip.SpaceShipStartSpacePointsCosts;

        if (OrderAvailable && SpaceShipAvailable && SpacePointsAvailableForStart)
        {
            orderDispatcher.CelestialBody = this;
            orderDispatcher.ResourceTransferOrders = ResourceTransferOrders;
            orderDispatcher.DispatchOrders();
        }
    }

    
    public void CheckResourceSurplus()
    {
        //Check if there is a ressource surplus and send it to the system. 
    }
    public void CheckResourceDeficit()
    {
        //Check if there is a shortage of resources and balance it out of the system if necessary.
    }
    //List<Structure> GetAllowedStructures()
    //{
    //    return 
    //}

    abstract public void UpdateInfoText();



}
