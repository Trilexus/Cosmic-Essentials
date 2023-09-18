using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public Resource Resource;
    public int EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
    
    // Konstruktor
    public Structure(string name, StructureType type, Resource resource, int ecoImpactFactor, List<AllowedLocation> allowedLocations)
    {
        this.Name = name;   
        this.Type = type;
        this.Resource = resource;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
    }

    public Structure(StructureScriptableObject data)
    {
        this.Name = data.Name;
        this.Type = data.Type;
        this.Resource = data.Resource;
        this.EcoImpactFactor = data.EcoImpactFactor;
        this.AllowedLocations = data.AllowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
    }

    public bool IsLocationAllowed(AllowedLocation location)
    {
        return AllowedLocations.Contains(location);
    }
}
