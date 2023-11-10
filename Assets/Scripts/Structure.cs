using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public List <Resource> Resources;
    public List<Resource> Costs;
    public int EcoImpactFactor;
    public List<LocationType> AllowedLocations;
    public GameObject Symbol;
    public int LivingSpace;
    
    // Konstruktor
    public Structure(string name, StructureType type, List<Resource> resources, List<Resource> costs, int ecoImpactFactor, List<LocationType> allowedLocations, GameObject Symbol, int LivingSpace)
    {
        this.Name = name;   
        this.Type = type;
        this.Resources = resources;
        this.Costs = costs;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<LocationType>(); // Verhindert NullReferenceException
        this.Symbol = Symbol;
        this.LivingSpace = LivingSpace;
    }

    public Structure(StructureScriptableObject data)
    {
        this.Name = data.Name;
        this.Type = data.Type;
        this.Resources = data.Resources;
        this.Costs = data.Costs;
        this.EcoImpactFactor = data.EcoImpactFactor;
        this.AllowedLocations = data.AllowedLocations ?? new List<LocationType>(); // Verhindert NullReferenceException
        this.Symbol = data.Symbol;
        this.LivingSpace = data.LivingSpace;
    }

    public bool IsLocationAllowed(List<LocationType> locations)
    {
        bool isAllowed = AllowedLocations.Intersect(locations).Any();
        return isAllowed;
    }

    public bool IsLocationAllowed(LocationType location)
    {
        bool isAllowed = AllowedLocations.Contains(location);
        return isAllowed;
    }

    public bool AreResourcesSufficientForStructure(CelestialBody celestialBody) 
    {
       foreach (Resource resource in Costs)
        {
            if (celestialBody.ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity < resource.Quantity)
            {
                return false;
            }
        }
        return true;
    }

}
