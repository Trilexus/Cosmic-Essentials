using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "ScriptableObjects/StructureScriptableObject")]
public class StructureScriptableObject : ScriptableObject
{
    public string Name;
    public StructureType Type;
    public List<Resource> Resources;
    public float EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
}
