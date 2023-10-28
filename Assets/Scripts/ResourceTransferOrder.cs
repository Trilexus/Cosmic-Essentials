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
    public CelestialBody Destination;
    public int Repetitions;
    public bool IsPrioritized;
    public bool ReturnToOrigin;
    public bool OnlyFullShipment;

    public ResourceTransferOrder(List<ResourceStorage> resourceShipmentDetails, CelestialBody origin, CelestialBody destination, int repetitions, bool isPrioritized, bool onlyFullShipment, bool ReturnToOrigin)
    {
        ResourceShipmentDetails = resourceShipmentDetails;
        Origin = origin;
        Destination = destination;
        Repetitions = repetitions;
        IsPrioritized = isPrioritized;
        this.ReturnToOrigin = ReturnToOrigin;
        OnlyFullShipment = onlyFullShipment;
    }

}
