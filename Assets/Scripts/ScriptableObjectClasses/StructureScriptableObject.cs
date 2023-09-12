using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructure", menuName = "ScriptableObjects/StructureScriptableObject")]
public class StructureScriptableObject : ScriptableObject
{
    public string Name;
    public StructureType Type;
    public Resource Resource;
    public int EcoImpactFactor;
    public List<AllowedLocation> AllowedLocations;
}
