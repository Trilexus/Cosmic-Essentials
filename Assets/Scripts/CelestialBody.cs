using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TMPro;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

abstract public class CelestialBody : MonoBehaviour
{
    public string Name;
    [SerializeField]
    public int maxAreas;
    [SerializeField]
    public Population population;
    [SerializeField]
    public float interval = 2f; // Zeit zwischen zwei Ticks, wird durch ProductivityRate beeinflusst
    // Every Celestial Body have a default production of ressources.
    [SerializeField]
    public List<Resource> BaseResourceProduction;
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
    public LocationType AllowedLocation;
    //public List<ResourceStorage> ResourceStorageCelestialBody = new List<ResourceStorage>();
    public StringBuilder sb = new StringBuilder();
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody = new Dictionary<ResourceType, ResourceStorage>();
    public List<Resource> StartResources = new List<Resource>();
    
    public List<ResourceTransferOrder> ResourceTransferOrders = new List<ResourceTransferOrder>();
    [SerializeField]
    private List<ModifierScriptableObject> modifierScriptableObjects;
    [SerializeField]
    public List<Modifier> modifiers;
    //public int SpaceShipTransporterAvailable = 0;
    public int CurrentResourceStorageLimit;
    public int DefaultResourceStorageLimit = 5000;

    public List <SpaceShip> SpacecraftReadyForUnloading = new List<SpaceShip>();
    public List<HangarSlot> Hangar = new List<HangarSlot>();
    protected HangarManager hangarManager = new HangarManager();
    public const int OneHundredPercent = 100;

    ResourceTransferDispatcher orderDispatcher;
    public int MaxOrderLoops = 5;


    [SerializeField]
    protected TextMeshProUGUI celestialBodyName;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoTop;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoLeft;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoRight;

    public virtual void Start()
    {
        celestialBodyName.text = Name;
        interval = Random.Range(1.5f, 2.5f);
        interval = (float)Math.Round(interval, 2);
        InitializeModifiers();
        InitializeResources();
        nextTickTime = Time.time + interval;
        StartCoroutine(TickCoroutine());
        InitializeStartingBuildings();
        orderDispatcher = new ResourceTransferDispatcher(ResourceStorageCelestialBody);
        for (int i = 0; i <= 2; i++)
        {
            Tick();
        }
    }
    public void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceStorageCelestialBody[type] = new ResourceStorage(type.ToString(), CurrentResourceStorageLimit, 0, 0, 0);
        }
        foreach (var resource in StartResources)
        {
            ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity += resource.Quantity;
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
    private void InitializeModifiers()
    {
        modifiers = ModifierManager.Instance.CreateModifiers(modifierScriptableObjects);
    }
    protected virtual void Update()
    {
        // Aktualisiere die verbleibende Zeit
        timeUntilNextTick = nextTickTime - Time.time;
        // Aktualisiere die Anzeige
        TimeToTick.SetText(timeUntilNextTick.ToString("0.0") + "/" + interval);
    }

    public void AddResourceTransferOrder(ResourceTransferOrder order)
    {
        //ResourceTransferOrders.Add(order);
        ResourceTransferOrders.Insert(0, order);
        if (GUIManager.Instance.selectedCelestialBodyScript == this)
        {
            GUIManager.Instance.FillOrdersOverview();
        }
    }
    public void RemoveTranferOrder(ResourceTransferOrder order)
    {
        ResourceTransferOrders.Remove(order);
        if (GUIManager.Instance.ActiveCelestialBodyTarget == this)
        {
            GUIManager.Instance.FillOrdersOverview();
        }
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
        CalculateCtructureEffekts();
        ManageResourceProduction();
        UnloadTheSpaceShips();
        ProcessResourceTransferOrders();
        //GUIManager.Instance.UpdateTopPanelInfos();
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
        if (areaToRemove != null)
        {
            Areas.Remove(areaToRemove);
        }

    }

    private void ExecuteConstructionProgress()
    {
        ExecuteConstructionPrograssStructure();
        EcecuteConstructionProgressHangarSlot();
    }

    private void ExecuteConstructionPrograssStructure()
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
    }

    private void EcecuteConstructionProgressHangarSlot()
    {
        int countOfConstructingSpacefleet = hangarManager.GetSpaceFleetCount(false);
        if (countOfConstructingSpacefleet > 0)
        {
            float individualConstructionRate = ConstructionRate / countOfConstructingSpacefleet;
            individualConstructionRate = individualConstructionRate * 100 * ProductivityRate;
            hangarManager.GetHangarSlots(false).ToList().ForEach(slot => slot.AddTooConstructionStatus((int)individualConstructionRate));
        }
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
            foreach (ResourceStorage resource in spaceship.ResourceStorage.Values)
            {
                ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity += resource.StorageQuantity;
                resource.StorageQuantity = 0; //SpaceShip ist nun leer.
            }
            if (spaceship.ReturnToOrigin)
            {
                spaceship.StartJourney(this, spaceship.origin, false);
            }else
            {
                SpaceShipPool.Instance.ReturnSpaceShipToPool(spaceship.gameObject);
                //SpaceShipTransporterAvailable += 1; //SpaceShip ist nun auf diesem CelestialBody verfügbar.
                hangarManager.AddSpaceShip(spaceship);
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
        // Buffs und Debuffs werden hier angewendet.
        ModifierManager.Instance.ApplyModifiers(modifiers, ResourceStorageCelestialBody);
        
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
        //bool SpacePointsAvailableForStart = ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity > SpaceShip.StartSpacePointsCosts;
        if (OrderAvailable)
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

    public virtual void UpdateInfoText()
    {

    }

    public virtual int GetResourceCount(ResourceType resourceType, DataTypeResource dataType)
    {
        if (ResourceStorageCelestialBody.ContainsKey(resourceType))
        {
            switch (dataType)
            {
                case DataTypeResource.Production:
                    return ResourceStorageCelestialBody[resourceType].ProductionQuantity;
                case DataTypeResource.Consumption:
                    return ResourceStorageCelestialBody[resourceType].ConsumptionQuantity;
                case DataTypeResource.Storage:
                    return ResourceStorageCelestialBody[resourceType].StorageQuantity;
                default:
                    return 0;
            } 
        } else
        {
            return 0;
        }
    }

    public virtual int GetStructuresCount(StructureType structureType, bool isCompleted)
    {
        switch (isCompleted)
        {
            case true:
                return Areas.Count(x => x.structure.Type == structureType && x.constructionProgress >= 100);
            case false:
                return Areas.Count(x => x.structure.Type == structureType && x.constructionProgress < 100);
        }
    }

    public virtual void CalculateCtructureEffekts()
    {
        CalculateStructureStorageEffekt();
    }

    public void CalculateStructureStorageEffekt()
    {
        CurrentResourceStorageLimit = DefaultResourceStorageLimit;
        Areas.Where(x => x.constructionProgress >= 100).ToList().ForEach(x => CurrentResourceStorageLimit += x.structure.StorageCapacity);
        ResourceStorageCelestialBody.Values.ToList().ForEach(x => x.MaxQuantity = CurrentResourceStorageLimit);
    }

    public void InitiateConstructionSpacefleet(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        int constructionProgress = 0;
        AddShipToHangar(new HangarSlot(spacefleetScriptableObject, constructionProgress));
    }
    public void AddShipToHangar(HangarSlot slot)
    {
        hangarManager.AddHangarSlot(slot);
    }


    public int GetSpaceFleetCount(SpacefleetScriptableObject spacefleetScriptableObject, bool isCompleted)
    {
        return hangarManager.GetSpaceFleetCount(spacefleetScriptableObject, isCompleted);
    }

    public void SubscribeToHangarChanges(HangarManager.HangarChangeHandler handler)
    {
        hangarManager.OnHangarSlotChanged += handler;
    }
    public void UnSubscribeToHangarChanges(HangarManager.HangarChangeHandler handler)
    {
        hangarManager.OnHangarSlotChanged -= handler;
    }

    public TResult PerformHangarOperation<TResult>(Func<HangarManager, TResult> operation)
    {
        return operation(hangarManager);
    }
}
public enum DataTypeResource
{
    Production,
    Consumption,
    Storage
}
