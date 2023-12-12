using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public List<ResourceScriptableObject> resources;
    public List<ResourceScriptableObject> Resources
    {
        get
        {
            List<ResourceScriptableObject> returnList = new List<ResourceScriptableObject>();
            foreach (var resource in resources)
            {
                returnList.Add(AddUpgrades(resource));
            }
            return returnList;
        }
        set
        {
            resources = value;
        }
    }
    public List<ResourceScriptableObject> Costs;
    public int EcoImpactFactor;
    public List<LocationType> AllowedLocations;
    public GameObject Symbol;
    public int LivingSpace;
    public int StorageCapacity;
    public int CostsPopulation;
    public Sprite Sprite;
    public List<StructureResourceUpgrade> Upgrades;


    private ResourceScriptableObject AddUpgrades(ResourceScriptableObject resource)
    {
        ResourceScriptableObject returnResource = ScriptableObject.CreateInstance<ResourceScriptableObject>();
        returnResource.Quantity = resource.Quantity;
        returnResource.ResourceType = resource.ResourceType;

        foreach (var upgrade in Upgrades)
        {
            if (upgrade.ResourcesType.Contains(resource.ResourceType))
            {
                returnResource.Quantity += upgrade.ResourcesChangeAmount;
            }
        }
        return returnResource;
    }

    // Konstruktor
    public Structure(string name, StructureType type, List<ResourceScriptableObject> resources, List<ResourceScriptableObject> costs, int ecoImpactFactor, List<LocationType> allowedLocations, GameObject Symbol, int StorageCapacity, int LivingSpace)
    {
        this.Name = name;   
        this.Type = type;
        this.Resources = resources;
        this.Costs = costs;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<LocationType>(); // Verhindert NullReferenceException
        this.Symbol = Symbol;
        this.StorageCapacity = StorageCapacity;
        this.LivingSpace = LivingSpace;
        Upgrades = new List<StructureResourceUpgrade>();
    }

    public Structure(StructureScriptableObject data)
    {
        this.Name = data.Name;
        this.Type = data.Type;
        this.Resources = data.Resources;
        this.Costs = data.Costs;
        this.CostsPopulation = data.CostsPopulation;
        this.EcoImpactFactor = data.EcoImpactFactor;
        this.AllowedLocations = data.AllowedLocations ?? new List<LocationType>(); // Verhindert NullReferenceException
        this.Symbol = data.Symbol;
        this.StorageCapacity = data.StorageCapacity;
        this.LivingSpace = data.LivingSpace;
        this.Sprite = data.Sprite;
        Upgrades = new List<StructureResourceUpgrade>();
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
       foreach (ResourceScriptableObject resource in Costs)
        {
            if (celestialBody.ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity < resource.Quantity)
            {
                return false;
            }
        }
        return true;
    }

}
