using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpacefleetObject", menuName = "ScriptableObjects/SpacefleetScriptableObject")]
public class SpacefleetScriptableObject : ScriptableObject
{
    public string Name;
    public SpacefleetType Type;
    public List<Resource> Costs;
    public List<StructureScriptableObject> StructureRequirements;
    public GameObject Prefab;
    public List<Resource> Resources;
    public int EcoImpactFactor;
    public List<LocationType> AllowedLocations;
    public GameObject Symbol;
    public Sprite Sprite;
    public int LivingSpace;
}
