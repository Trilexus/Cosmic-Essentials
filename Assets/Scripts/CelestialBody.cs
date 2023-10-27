using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

abstract public class CelestialBody : MonoBehaviour
{
    [SerializeField]
    protected int maxAreas;
    [SerializeField]
    protected Population population;
    
    public float interval = 2f; // Zeit zwischen zwei Ticks, wird durch ProductivityRate beeinflusst
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
        orderDispatcher = new ResourceTransferDispatcher(ResourceStorageCelestialBody);
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
        foreach (SpaceShip spaceship in SpacecraftReadyForUnloading)
        {
            foreach (ResourceStorage resource in spaceship.ResourceStorageSpaceShip.Values)
            {
                ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity += resource.StorageQuantity;
                resource.StorageQuantity = 0; //SpaceShip ist nun leer.
            }
            if (spaceship.FliesBack)
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
        if (ResourceTransferOrders.Count > 0)
        {
            int CountOrderRepetitions = 0;
            while (!ExecuteNextOrder() && CountOrderRepetitions++ <= MaxOrderLoops) ;
        }
    }

    public virtual bool ExecuteNextOrder()
    {
        if (SpaceShipTransporterAvailable > 0 && ResourceTransferOrders.Count > 0)
        {
            orderDispatcher.Order = ResourceTransferOrders.FirstOrDefault();
            SpaceShip spaceShip =  orderDispatcher.DispatchOrder();
            if (spaceShip != null)
            {
                SpaceShipTransporterAvailable--; // Ein SpaceShip wird für den Transport genutzt.
                spaceShip.StartJourney(orderDispatcher.Order.Origin, orderDispatcher.Order.Destination, orderDispatcher.Order.FliesBack); // SpaceShip starten.
            } else
            {
                return UpdateOrderStatus(false);
            }          
        }
        return UpdateOrderStatus(true);
    }

    public bool UpdateOrderStatus(bool isProcessed) // Order wurde bearbeitet, wird aus der Liste entfernt oder hinten angehängt. (true = bearbeitet, false = nicht bearbeitet)
    {
        if (isProcessed)
        {
            if (orderDispatcher.Order.Repetitions <= 0)
            {
                ResourceTransferOrders.Remove(orderDispatcher.Order); // Order aus der Liste entfernen.
            }
            else if (orderDispatcher.Order.IsPrioritized) //Order ist prioriziert, bleibt in der Liste an erster Stelle und wird erneut ausgeführt.
            {
                orderDispatcher.Order.Repetitions--; // Anzahl der Wiederholungen reduzieren.
            }
            else // Order ist nicht priorisiert, wird hinten an die Liste angehängt und erneut ausgeführt.
            {
                orderDispatcher.Order.Repetitions--; // Anzahl der Wiederholungen reduzieren.
                ResourceTransferOrders.Remove(orderDispatcher.Order); // Order aus der Liste entfernen.
                ResourceTransferOrders.Add(orderDispatcher.Order); // Order wieder hinten an die Liste anhängen.
            }
            return true; // Order ist priorisiert, bleibt in der Liste an erster Stelle und wird erneut ausgeführt.
        } else if (!orderDispatcher.Order.IsPrioritized) // Order ist nicht priorisiert, wird hinten an die Liste angehängt und erneut ausgeführt.
        {
            ResourceTransferOrders.Remove(orderDispatcher.Order); // Order aus der Liste entfernen.
            ResourceTransferOrders.Add(orderDispatcher.Order); // Order wieder hinten an die Liste anhängen.
            return false; // Order ist nicht priorisiert, bleibt in der Liste an letzter Stelle und wird erneut ausgeführt.
        }
        return true; // Order ist priorisiert, bleibt in der Liste an erster Stelle und wird erneut ausgeführt.
    }


    public bool CanFulfillOrder(ResourceTransferOrder order)
    {
        foreach (ResourceStorage shipmentDetail in order.ResourceShipmentDetails)
        {
            // Überprüfe, ob der ResourceType im Dictionary existiert
            if (!ResourceStorageCelestialBody.TryGetValue(shipmentDetail.ResourceType, out var storage))
            {
                return false; // ResourceType nicht gefunden, Order kann nicht erfüllt werden
            }
            // Überprüfe, ob die Menge im Dictionary größer oder gleich der geforderten Menge ist
            if (storage.StorageQuantity < shipmentDetail.StorageQuantity)
            {
                return false; // Nicht genug Ressourcen, Order kann nicht erfüllt werden
            }
        }
        return true; // Alle Bedingungen erfüllt, Order kann erfüllt werden
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
