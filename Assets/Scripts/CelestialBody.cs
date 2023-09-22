using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

abstract public class CelestialBody : MonoBehaviour
{
    public int area;
    public float interval = 5f;
    public float ProductivityRate = 1f; //smaler = faster
    public float ConstructionRate = 0.2f; //bigger = faster
    [SerializeField]
    public List<Area> Areas = new List<Area>();
    public AllowedLocation allowedLocation;
    private List<Resource> ResourceStorage;

    public void Start()
    {
        StartCoroutine(TickCoroutine());
    }

    private IEnumerator TickCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval * ProductivityRate);
            Tick();
        }
    }

    private void Tick()
    {
        //Debug.Log("Tick: " + gameObject.name + " - "+ Time.time);
        ExecuteConstructionProgress();
        ManageResourceProduction();
    }

    public void OnMouseDown()
    {
        Debug.Log("Clicked: " + gameObject.name);
        GUIManager.Instance.ShowCelestialBodyMenu(gameObject);
    }
    public void InitiateConstructionStructure(Structure structure)
    {
            //Start building projects (farm, power plant, mine, research center).
            if (area > Areas.Count)
        {
            Debug.Log("Building");
            Areas.Add(new Area { structure = structure, constructionProgress = 0 });
            Debug.Log("Build: " + Areas.Last().structure.Name);
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
        // Combine buildings and other factors and calculate the production of resources.
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



}
