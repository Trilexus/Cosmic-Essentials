using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "ScriptableObjects/StructureScriptableObject")]
public class StructureScriptableObject : EntityScriptableObject
{
    public StructureType Type;
    public List<Resource> Resources;
    public int StorageCapacity;
    public int EcoImpactFactor;
    public List<LocationType> AllowedLocations;


}
