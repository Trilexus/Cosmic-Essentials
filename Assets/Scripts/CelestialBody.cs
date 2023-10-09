using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager.Requests;
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
    [SerializeField]
    public List<ResourceStorage> ResourceStorageCelestialBody = new List<ResourceStorage>
    {
        new ResourceStorage("Food", 0, 1000),
        new ResourceStorage("Metal", 0, 1000),
        new ResourceStorage("Energy", 0, 1000),
        new ResourceStorage("SpacePoints", 0, 1000)
    };
    public List<ResourceStorage> ResourceProductionCelestialBody = new List<ResourceStorage>
    {
        new ResourceStorage("Food", 0, 10000),
        new ResourceStorage("Metal", 0, 10000),
        new ResourceStorage("Energy", 0, 10000),
        new ResourceStorage("SpacePoints", 0, 10000)
    };
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoTop;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoLeft;
    [SerializeField]
    protected TextMeshProUGUI celestialBodyInfoRight;

    public virtual void Start()
    {
        nextTickTime = Time.time + interval;
        StartCoroutine(TickCoroutine());
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


    private void ManageResourceProduction()
    {
        ResetResourceProduction();
        // Add the base production of resources to the resource storage. Base production is the production of resources without buildings and other factors. It isn't affected by the productivity rate.
        foreach ( Resource resource in BaseResourceProduction)
        {
            ResourceStorageCelestialBody.Where(x => x.Name == resource.Name).First().Quantity += resource.Quantity;
            ResourceProductionCelestialBody.Where(x => x.Name == resource.Name).First().Quantity += resource.Quantity;
        }

        // Combine buildings and other factors and calculate the production of resources.
        foreach (var area in Areas)
        {
            if (area.constructionProgress >= 100)
            {
                foreach (var resource in area.structure.Resources)
                {
                    int newProduction;
                    if (resource.Quantity > 0)
                    {
                        newProduction = (int)(resource.Quantity * ProductivityRate);
                    } else
                    {
                        newProduction = resource.Quantity;
                    }
                    ResourceProductionCelestialBody.Where(x => x.Name == resource.Name).First().Quantity += newProduction;
                    var targetResource = ResourceStorageCelestialBody.Where(x => x.Name == resource.Name).First();
                    targetResource.Quantity = targetResource.Quantity + newProduction;

                }
            }
        }

        //Population consumes food and increase the productivity rate.
        ResourceStorageCelestialBody.Where(x => x.Name == "Food").First().Quantity -= population.CurrentPopulation;

        foreach (var resourceStorage in ResourceStorageCelestialBody)
        {
            resourceStorage.Quantity = Math.Clamp(resourceStorage.Quantity, minimalResourceStorageQuantity, resourceStorage.MaxQuantity);
        }

    }

    private void ResetResourceProduction()
    {
        foreach (var resource in ResourceProductionCelestialBody)
        {
            resource.Quantity = 0;
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
