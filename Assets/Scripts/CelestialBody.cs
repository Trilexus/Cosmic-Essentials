using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;

abstract public class CelestialBody : MonoBehaviour
{
    [SerializeField]
    protected int maxAreas;
    [SerializeField]
    protected Population population;
    
    public float interval = 5f; // Zeit zwischen zwei Ticks, wird durch ProductivityRate beeinflusst
    // Every Celestial Body have a default production of ressources.
    [SerializeField]
    private List<Resource> BaseResourceProduction;

    public float ProductivityRateBasicValue = 1f;
    public float ProductivityRate = 1f; 
    //Minimalwerte für die ProductivityRate
    public float MinProductivityRate = 0.5f;
    //Maximalwerte für die ProductivityRate
    public float MaxProductivityRate = 3f;
    private float nextTickTime; // Zeitpunkt des nächsten Ticks
    private float timeUntilNextTick; // Verbleibende Zeit bis zum nächsten Tick
    public TextMeshProUGUI TimeToTick;
    public float ConstructionRate = 0.2f; //bigger = faster
    [SerializeField]
    public List<Area> Areas = new List<Area>();
    public AllowedLocation allowedLocation;
    int minimalResourceStorageQuantity = -1;
    //public List<ResourceStorage> ResourceStorageCelestialBody = new List<ResourceStorage>();
    public StringBuilder sb = new StringBuilder();
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody = new Dictionary<ResourceType, ResourceStorage>();
    public List<ResourceTransferOrder> ResourceTransferOrders = new List<ResourceTransferOrder>();
    public int SpaceShipTransporterAvailable = 0;
    public List <SpaceShip> SpacecraftReadyForUnloading = new List<SpaceShip>();
    public const int OneHundredPercent = 100;

    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoTop;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoLeft;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoRight;

    public virtual void Start()
    {
        InitializeResources();
        nextTickTime = Time.time + interval;
        StartCoroutine(TickCoroutine());
    }
    public void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceStorageCelestialBody[type] = new ResourceStorage(type.ToString(), 1000, 0, 0, 0);
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
        DispatchShipsForResourceTransfer();
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
        var areaToRemove = Areas.LastOrDefault(x => x.structure == structure);
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
                 .ForEach(x => x.constructionProgress += (int)(individualConstructionRate * 100));
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
        foreach (SpaceShip spacehip in SpacecraftReadyForUnloading)
        {
            foreach (ResourceStorage resource in spacehip.ResourceStorageSpaceShip.Values)
            {
                ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity += resource.StorageQuantity;
            }
            SpaceShip_TransporterMisslePool.Instance.ReturnSpaceShipToPool(spacehip.gameObject);
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
    
    public virtual void DispatchShipsForResourceTransfer()
    {

        if (SpaceShipTransporterAvailable > 0 && ResourceTransferOrders.Count > 0)
        {
            Debug.Log("SpaceShip und Order vorhanden");
            SpaceShip spaceShip = SpaceShip_TransporterMisslePool.Instance.GetPooledSpaceShip().GetComponent<SpaceShip>(); //SpaceShip aus dem Pool holen.
            if (spaceShip.SpaceShipCosts <= ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity)
            {
                ResourceTransferOrder firstOrder = ResourceTransferOrders.FirstOrDefault(); // Erste Order aus der Liste holen.
                CelestialBody targetCelestialBody = firstOrder.Destination; // Ziel des SpaceShips holen.
                CelestialBody startCelestialBody = firstOrder.Origin; // Start des SpaceShips holen.
                foreach (ResourceStorage resourceOrder in firstOrder.ResourceShipmentDetails)
                {
                    ResourceType currentType = resourceOrder.ResourceType; // Typ der Order holen.
                    int orderStorage = resourceOrder.StorageQuantity; // Menge der Order holen.

                    int celestialBodyStorage = ResourceStorageCelestialBody[currentType].StorageQuantity; // Menge der Ressource auf dem Planeten holen.
                    if (currentType.Equals(ResourceType.SpacePoints)) // SpacePoints werden nicht vom Planeten abgezogen.
                    {
                        celestialBodyStorage -= spaceShip.SpaceShipCosts; // Kosten für den Transport werden subtrahiert.
                    }
                    int AvailableResourceQuantity = Math.Min(orderStorage, celestialBodyStorage); // Minimale Menge der Ressource berechnen.
                    AvailableResourceQuantity = Math.Min(AvailableResourceQuantity, spaceShip.maxStorage); // Minimale Menge der Ressource berechnen.
                    
                    Debug.Log("Verladen:" + spaceShip.FreeStorageSpace + "/" + spaceShip.maxStorage);

                    if (AvailableResourceQuantity > 0) // 
                    {
                        spaceShip.FreeStorageSpace -= AvailableResourceQuantity; // Lagerkapazität des SpaceShips reduzieren.
                        spaceShip.ResourceStorageSpaceShip[currentType].StorageQuantity = AvailableResourceQuantity; // Menge der Ressource im SpaceShip setzen.
                        ResourceStorageCelestialBody[currentType].StorageQuantity -= AvailableResourceQuantity; // Menge der Ressource auf dem Planeten reduzieren.
                    }
                    if (spaceShip.FreeStorageSpace <= 0) // SpaceShip voll, dann abbrechen.
                    {
                        break; // SpaceShip voll, dann abbrechen.
                    }

                }
                if (spaceShip.FreeStorageSpace < spaceShip.maxStorage) // SpaceShip startet nur, wenn etwas verladen wurde.
                {
                    Debug.Log("SpaceShip startet");
                    ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity -= spaceShip.SpaceShipCosts; // Kosten für den Transport werden abgezogen.
                    SpaceShipTransporterAvailable--; // Ein SpaceShip wird für den Transport genutzt.
                    ResourceTransferOrders.Remove(firstOrder); // Order aus der Liste entfernen.
                    spaceShip.StartJourney(startCelestialBody, targetCelestialBody); // SpaceShip starten.
                }
            } else
            {
                SpaceShip_TransporterMisslePool.Instance.ReturnSpaceShipToPool(spaceShip.gameObject);
            }

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
