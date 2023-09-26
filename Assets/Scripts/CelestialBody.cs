using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using TMPro;

abstract public class CelestialBody : MonoBehaviour
{
    [SerializeField]
    protected int maxAreas;
    [SerializeField]
    protected float population;
    
    public float interval = 5f;
    public float ProductivityRate = 1f; //smaler = faster
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
        TimeToTick.SetText(timeUntilNextTick.ToString("0.0"));
    }

    private IEnumerator TickCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval * ProductivityRate);
            nextTickTime = Time.time + interval * ProductivityRate; // Setze den Zeitpunkt des nächsten Ticks
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
        GUIManager.Instance.MoveCelestialBodyMenu(gameObject);
    }
    public void InitiateConstructionStructure(Structure structure)
    {
            //Start building projects (farm, power plant, mine, research center).
            if (maxAreas > Areas.Count)
        {
            Areas.Add(new Area { structure = structure, constructionProgress = 0 });
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

    public void DemolishStructure()
    {
        //Start demolish structure
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
