using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScriptableObject : ScriptableObject
{
    public string Name;
    public List<ResourceScriptableObject> Costs; // Welche Ressourcen braucht es zum Bau?
    public List<StructureScriptableObject> StructureRequirements; // Welche Geb�ude braucht es zum Bau?
    public List<ResearchNodeScriptableObject> ResearchRequirements; // Welche Forschungen braucht es zum Bau?
    public int CostsPopulation; // Wie viele Leute braucht es zum Betrieb?
    public int LivingSpace; // Wie viel Wohnraum bietet es?
    public GameObject Prefab;
    public List<LocationType> Locations;
    public GameObject Symbol;
    public Sprite Sprite;
}
