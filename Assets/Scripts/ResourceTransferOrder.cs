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

    public ResourceTransferOrder(List<ResourceStorage> ResourceShipmentDetails, CelestialBody origin, CelestialBody destination)
    {
        this.ResourceShipmentDetails = ResourceShipmentDetails;
        this.Origin = origin;
        this.Destination = destination;
    }
   
}
