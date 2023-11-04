using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Structure
{
    public string Name;
    public StructureType Type;
    public List <Resource> Resources;
    public List<Resource> Costs;
    public int EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
    public GameObject Symbol;
    public int LivingSpace;
    
    // Konstruktor
    public Structure(string name, StructureType type, List<Resource> resources, List<Resource> costs, int ecoImpactFactor, List<AllowedLocation> allowedLocations, GameObject Symbol, int LivingSpace)
    {
        this.Name = name;   
        this.Type = type;
        this.Resources = resources;
        this.Costs = costs;
        this.EcoImpactFactor = ecoImpactFactor;
        this.AllowedLocations = allowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
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
        this.AllowedLocations = data.AllowedLocations ?? new List<AllowedLocation>(); // Verhindert NullReferenceException
        this.Symbol = data.Symbol;
        this.LivingSpace = data.LivingSpace;
    }

    public bool IsLocationAllowed(AllowedLocation location)
    {
        return AllowedLocations.Contains(location);
    }
}
