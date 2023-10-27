using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class ResourceTransferDispatcher
{
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody = new Dictionary<ResourceType, ResourceStorage>();
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBodyWithoutStartingCosts;
    public ResourceTransferOrder Order;
    private SpaceShip spaceShip;


    public ResourceTransferDispatcher(Dictionary<ResourceType, ResourceStorage> resourceStorageCelestialBody)
    {
        ResourceStorageCelestialBody = resourceStorageCelestialBody;
    }

    public SpaceShip DispatchOrder()
    {
        Debug.Log("DispatchOrder");
        if (!CanFulfillOrder()) return null; // Wenn die Order nicht erf�llt werden kann, ist hier Schluss  
        Debug.Log("CanFulfillOrder");
        spaceShip = SpaceShip_TransporterMisslePool.Instance.GetPooledSpaceShip().GetComponent<SpaceShip>(); //SpaceShip aus dem Pool holen.
        return LoadSpaceShip(); 
    }

    public SpaceShip LoadSpaceShip()
    {
        foreach (var orderResource in Order.ResourceShipmentDetails)
        {
            ResourceType currentType = orderResource.ResourceType; // Typ der Order holen.
            int orderStorage = orderResource.StorageQuantity; // Menge der Order holen.
            int celestialBodyStorage = ResourceStorageCelestialBody[currentType].StorageQuantity; // Menge der Ressource auf dem Planeten holen.
            int AvailableResourceQuantity = Math.Min(orderStorage, celestialBodyStorage); // Minimale Menge der Ressource berechnen.
            ResourceStorageCelestialBody[orderResource.ResourceType].StorageQuantity -= AvailableResourceQuantity; // Ressourcen auf dem Planeten reduzieren.
            spaceShip.FreeStorageSpace -= AvailableResourceQuantity; // Lagerkapazit�t des SpaceShips reduzieren.
            spaceShip.ResourceStorageSpaceShip[orderResource.ResourceType].StorageQuantity += AvailableResourceQuantity; // Ressourcen im SpaceShip erh�hen.
            if (spaceShip.FreeStorageSpace <= 0) // SpaceShip voll, dann abbrechen.
            {
                break;
            }
        }
        if (spaceShip.FreeStorageSpace == spaceShip.maxStorage)
        {
            return null; // SpaceShip leer, dann abbrechen.
        }
        else
        {
            return spaceShip; // SpaceShip nicht leer, dann weitermachen.
        }
    }


    public bool CanFulfillOrder()
    {
        var spacePointsResource = Order.ResourceShipmentDetails.FirstOrDefault(resource => resource.Name.Equals("SpacePoints"));
        int SpacePointsOrdered = spacePointsResource?.StorageQuantity ?? 0;
        int SpacePointsAvailable = ResourceStorageCelestialBody.TryGetValue(ResourceType.SpacePoints, out var spacePointsStorage) ? spacePointsStorage.StorageQuantity : 0;
        Debug.Log("SpacePointsOrdered: " + SpacePointsOrdered + "SpacePointsAvailable: " + SpacePointsAvailable);
        // �berpr�fe, ob genug SpacePoints vorhanden sind f�r Startkosten und Order (Vollst�ndig oder nicht)
        if (Order.OnlyFullShipment && SpacePointsAvailable < SpacePointsOrdered + SpaceShip.SpaceShipStartSpacePointsCosts)
        {
            Debug.Log("SpacePointsAvailable < SpacePointsOrdered + SpaceShip.SpaceShipStartSpacePointsCosts false");
            return false; // Wenn Order vollst�ndig ausgef�hrt werden soll und zu wenig SpacePoints f�r Order und Start vorhanden sind, Order kann nicht erf�llt werden
        }
        else if (SpacePointsAvailable < SpaceShip.SpaceShipStartSpacePointsCosts)
        {
            Debug.Log("SpacePointsAvailable < SpaceShip.SpaceShipStartSpacePointsCosts false");
            return false; // Nicht genug SpacePoints zum Start vorhanden sind, Order kann nicht erf�llt werden
        }
        else if (!Order.OnlyFullShipment)
        {
            Debug.Log("!Order.OnlyFullShipment true");
            return true; // Wenn die Order nicht vollst�ndig sein muss, ist sie immer erf�llbar, solange die Startkosten gedeckt sind
        }
        foreach (ResourceStorage shipmentDetail in Order.ResourceShipmentDetails)
        {
            // �berpr�fe, ob der ResourceType im Dictionary existiert
            if (!ResourceStorageCelestialBody.TryGetValue(shipmentDetail.ResourceType, out var storage))
            {
                Debug.Log("ResourceStorageCelestialBody.TryGetValue(shipmentDetail.ResourceType, out var storage) false");
                return false; // ResourceType nicht gefunden, Order kann nicht erf�llt werden
            }
            // �berpr�fe, ob die Menge im Dictionary gr��er oder gleich der geforderten Menge ist
            if (storage.StorageQuantity < shipmentDetail.StorageQuantity)
            {
                Debug.Log("storage.StorageQuantity < shipmentDetail.StorageQuantity false");
                return false; // Nicht genug Ressourcen, Order kann nicht erf�llt werden
            }
        }
        return true; // Alle Bedingungen erf�llt, Order kann erf�llt werden
    }
}
