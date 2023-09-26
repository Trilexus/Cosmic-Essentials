using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public List <Resource> Resources;
    public float EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
    
    // Konstruktor
    public Structure(string name, StructureType type, List<Resource> resources, int ecoImpactFactor, List<AllowedLocation> allowedLocations)
    {
        this.Name = name;   
        this.Type = type;
        this.Resources = resources;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
    }

    public Structure(StructureScriptableObject data)
    {
        this.Name = data.Name;
        this.Type = data.Type;
        this.Resources = data.Resources;
        this.EcoImpactFactor = data.EcoImpactFactor;
        this.AllowedLocations = data.AllowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
    }

    public bool IsLocationAllowed(AllowedLocation location)
    {
        return AllowedLocations.Contains(location);
    }
}
