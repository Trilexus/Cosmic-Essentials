using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public List <Resource> Resources;
    public int EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
    public GameObject Symbol;
    
    // Konstruktor
    public Structure(string name, StructureType type, List<Resource> resources, int ecoImpactFactor, List<AllowedLocation> allowedLocations, GameObject Symbol)
    {
        this.Name = name;   
        this.Type = type;
        this.Resources = resources;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
        this.Symbol = Symbol;
    }

    public Structure(StructureScriptableObject data)
    {
        this.Name = data.Name;
        this.Type = data.Type;
        this.Resources = data.Resources;
        this.EcoImpactFactor = data.EcoImpactFactor;
        this.AllowedLocations = data.AllowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
        this.Symbol = data.Symbol;
    }

    public bool IsLocationAllowed(AllowedLocation location)
    {
        return AllowedLocations.Contains(location);
    }
}
