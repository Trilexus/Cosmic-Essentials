using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceTransferOrder
{
    public ResourceType Type;
    public int Quantity;
    public int SpacePointCosts;
    public CelestialBody Origin;
    public CelestialBody Destination;

    public ResourceTransferOrder(ResourceType type, int quantity, int spacePointCosts, CelestialBody origin, CelestialBody destination)
    {
        this.Type = type;
        this.Quantity = quantity;
        this.SpacePointCosts = spacePointCosts;
        this.Origin = origin;
        this.Destination = destination;
    }
   
}
