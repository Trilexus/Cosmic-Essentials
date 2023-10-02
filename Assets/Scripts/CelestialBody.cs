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
    public float ProductivityRateBasicValue = 1f;
    public float ProductivityRate = 1f; //smaler = faster
    public float MaxProductivityRate = 3f;
    public float MinProductivityRate = 0.3f;
    private float nextTickTime; // Zeitpunkt des nächsten Ticks
    private float timeUntilNextTick; // Verbleibende Zeit bis zum nächsten Tick
    public TextMeshProUGUI TimeToTick;
    public float ConstructionRate = 0.2f; //bigger = faster
    [SerializeField]
    public List<Area> Areas = new List<Area>();
    public AllowedLocation allowedLocation;
    protected List<Resource> ResourceStorage = new List<Resource>
    {
        new Resource("Food", 0),
        new Resource("Metal", 0),
        new Resource("Energy", 0)
    };
    protected TextMeshProUGUI celestialBodyInfo;

    public virtual void Start()
    {
        nextTickTime = Time.time + interval * ProductivityRate;
        StartCoroutine(TickCoroutine());
    }

    protected virtual void Update()
    {
        // Aktualisiere die verbleibende Zeit
        timeUntilNextTick = nextTickTime - Time.time;
        // Aktualisiere die Anzeige
        TimeToTick.SetText(timeUntilNextTick.ToString("0.0") + "/" + interval * ProductivityRate);
    }

    private IEnumerator TickCoroutine()
    {
        while (true)
        {
            nextTickTime = Time.time + interval * ProductivityRate; // Setze den Zeitpunkt des nächsten Ticks
            yield return new WaitForSeconds(interval * ProductivityRate);
            Tick();
        }
    }

    protected virtual void Tick()
    {
        ExecuteConstructionProgress();
        ManageResourceProduction();
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
        var areaToRemove = Areas.FirstOrDefault(x => x.structure == structure);
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
        }
        //Executes building projects over time (farm, power plant, mine, research center).
    }


    private void ManageResourceProduction()
    {
        ResetResourceStorage();
        // Combine buildings and other factors and calculate the production of resources.
        foreach (var area in Areas)
        {
            if (area.constructionProgress >= 100)
            {
                foreach (var resource in area.structure.Resources)
                {
                    ResourceStorage.Where(x => x.Name == resource.Name).First().Quantity += resource.Quantity;
                }
            }
        }
        //Population consumes food
        ResourceStorage.Where(x => x.Name == "Food").First().Quantity -= population.CurrentPopulation;
    }

    private void ResetResourceStorage()
    {
        foreach (var resource in ResourceStorage)
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
