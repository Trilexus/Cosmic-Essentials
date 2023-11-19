using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceTransferOrder
{

    //public ResourceStorage ResourceShipmentDetails;
    public List<ResourceStorage> ResourceShipmentDetails = new List<ResourceStorage>();
    public CelestialBody Origin;
    public CelestialBody Target;
    public int Repetitions;
    public bool IsPrioritized;
    public bool ReturnToOrigin;
    public bool OnlyFullShipment;
    public bool IsForever;
    public bool AutoChooseShip;
    public SpacefleetScriptableObject SpacefleetScriptableObject;

    public ResourceTransferOrder(List<ResourceStorage> resourceShipmentDetails, CelestialBody origin, CelestialBody destination, int repetitions, bool isPrioritized, bool onlyFullShipment, bool ReturnToOrigin, bool isForever)
    {
        ResourceShipmentDetails = resourceShipmentDetails;
        Origin = origin;
        Target = destination;
        Repetitions = repetitions;
        IsPrioritized = isPrioritized;
        this.ReturnToOrigin = ReturnToOrigin;
        OnlyFullShipment = onlyFullShipment;
        IsForever = isForever;
        AutoChooseShip = true;
    }

    public ResourceTransferOrder(List<ResourceStorage> resourceShipmentDetails, CelestialBody origin, CelestialBody destination, int repetitions, bool isPrioritized, bool onlyFullShipment, bool ReturnToOrigin, bool isForever, SpacefleetScriptableObject spacefleetScriptableObject)
    {
        ResourceShipmentDetails = resourceShipmentDetails;
        Origin = origin;
        Target = destination;
        Repetitions = repetitions;
        IsPrioritized = isPrioritized;
        this.ReturnToOrigin = ReturnToOrigin;
        OnlyFullShipment = onlyFullShipment;
        IsForever = isForever;
        AutoChooseShip = false;
        SpacefleetScriptableObject = spacefleetScriptableObject;
    }

}
