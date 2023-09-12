using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

abstract public class CelestialBody : MonoBehaviour
{
    public int area;
    public List<Area> areas;
    public AllowedLocation allowedLocation;
    private List<Resource> ResourceStorage;


    public void InitiateConstructionStructure()
    {
        //Start building projects (farm, power plant, mine, research center).

    }

    private void ExecuteConstructionProgress()
    {
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
