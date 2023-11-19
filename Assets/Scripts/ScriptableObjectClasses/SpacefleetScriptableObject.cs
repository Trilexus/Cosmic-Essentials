using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpacefleetObject", menuName = "ScriptableObjects/SpacefleetScriptableObject")]
public class SpacefleetScriptableObject : EntityScriptableObject
{
    public SpacefleetType Type; // Enum, was für ein Schiff oder Raumstation ist es?
    public Dictionary<ResourceType, ResourceStorage> ResourceStorage = new Dictionary<ResourceType, ResourceStorage>();
    public int CargoSpace = 500;
    public int Speed; // Wie schnell ist es?
    public int Range; // Wie weit kann es fliegen?
    public int MaxFuel; // Wie viel Treibstoff kann es mitnehmen?
    public int Fuel; 
    public int StartSpacePointsCosts;


}
