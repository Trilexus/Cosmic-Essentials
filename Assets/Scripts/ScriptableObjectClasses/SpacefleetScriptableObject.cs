using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpacefleetObject", menuName = "ScriptableObjects/SpacefleetScriptableObject")]
public class SpacefleetScriptableObject : EntityScriptableObject
{
    public SpacefleetType Type; // Enum, was für ein Schiff oder Raumstation ist es?
    public Dictionary<ResourceType, ResourceStorage> ResourceStorage = new Dictionary<ResourceType, ResourceStorage>();
    public int CargoSpace = 500;
    int freeSpace;
    public int FreeSpace { get
        {
            freeSpace = CargoSpace;
            foreach (var resource in ResourceStorage)
            {
                freeSpace -= resource.Value.StorageQuantity;
            }
            return freeSpace;
        }
    }
        
    public int Speed; // Wie schnell ist es?
    public int Range; // Wie weit kann es fliegen?
    public int MaxFuel; // Wie viel Treibstoff kann es mitnehmen?
    public int Fuel; 
    public int StartSpacePointsCosts;

    private void OnEnable()
    {
        ResourceStorage.Add(ResourceType.Food, new ResourceStorage(ResourceType.Food, CargoSpace, 0, 0, 0));
        ResourceStorage.Add(ResourceType.Metal, new ResourceStorage(ResourceType.Metal, CargoSpace, 0, 0, 0));        
        ResourceStorage.Add(ResourceType.Energy, new ResourceStorage(ResourceType.Energy, CargoSpace, 0, 0, 0));
        ResourceStorage.Add(ResourceType.SpacePoints, new ResourceStorage(ResourceType.SpacePoints, CargoSpace, 0, 0, 0));
    }
}