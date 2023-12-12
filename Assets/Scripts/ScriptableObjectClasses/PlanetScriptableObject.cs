using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlanet", menuName = "ScriptableObjects/PlanetScriptableObject")]
public class PlanetScriptableObject : ScriptableObject
{
    public int area;
    public List<Area> areas;
    public LocationType allowedLocation;
    public List<ResourceScriptableObject> ResourceStorage;
}
