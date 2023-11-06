using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
  

public class ResourceTransferDispatcher
{
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody = new Dictionary<ResourceType, ResourceStorage>();
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBodyWithoutStartingCosts;
    public List<ResourceTransferOrder> ResourceTransferOrders = new List<ResourceTransferOrder>();
    public ResourceTransferOrder Order;
    public CelestialBody CelestialBody;
    private SpaceShip spaceShip;


    public ResourceTransferDispatcher()
    {
     
    }
    public ResourceTransferDispatcher(Dictionary<ResourceType, ResourceStorage> resourceStorageCelestialBody)
    {
        ResourceStorageCelestialBody = resourceStorageCelestialBody;
    }

    public void DispatchOrders()
    {
        foreach (var order in ResourceTransferOrders) // Alle Orders durchgehen.
        {
            Order = order; // Aktuelle Order setzen.
            bool canFulfillOrder = CanFulfillOrder(); // �berpr�fen, ob die Order erf�llt werden kann.
            if (canFulfillOrder)
            {
                spaceShip = SpaceShip_TransporterMisslePool.Instance.GetPooledSpaceShip().GetComponent<SpaceShip>(); //SpaceShip aus dem Pool holen.
                spaceShip.RefuelSpaceShip(order);
                LoadSpaceShip(); // SpaceShip beladen.
                if (!Order.IsForever)
                {
                    Order.Repetitions--; // Anzahl der Wiederholungen reduzieren.
                }
                spaceShip.StartJourney(Order.Origin,Order.Target,Order.ReturnToOrigin); // SpaceShip starten.
                CelestialBody.SpaceShipTransporterAvailable--; // SpaceShip auf dem Planeten reduzieren.
                break; // Order wurde erf�llt, deswegen wird die Schleife abgebrochen.
            }else if (Order.IsPrioritized)
            {
                break; // Order kann nicht erf�llt werden, ist aber priorisiert, deswegen wird die Schleife abgebrochen.
            }
        }
        if (Order.Repetitions == 0) // Order wurde erf�llt, deswegen wird die Order aus der Liste entfernt.
        {
            ResourceTransferOrders.Remove(Order);
            Order.Repetitions--; // Anzahl der Wiederholungen reduzieren, damit sie auch aus der GUI verschwindet.
        } else if (!Order.IsPrioritized) // Order an das Ende der Liste anh�ngen, wenn sie nicht priorisiert ist.
        {
            ResourceTransferOrders.Remove(Order); // Order aus der Liste entfernen.
            ResourceTransferOrders.Add(Order); // Order wieder hinten an die Liste anh�ngen.
        }
    }

    public void LoadSpaceShip()
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
    }




    public bool CanFulfillOrder()
    {
        var spacePointsResource = Order.ResourceShipmentDetails.FirstOrDefault(resource => resource.ResourceType == ResourceType.SpacePoints);
        int SpacePointsOrdered = spacePointsResource?.StorageQuantity ?? 0; 
        int SpacePointsAvailable = ResourceStorageCelestialBody.TryGetValue(ResourceType.SpacePoints, out var spacePointsStorage) ? spacePointsStorage.StorageQuantity : 0;
        int startCosts = SpaceShip.SpaceShipStartSpacePointsCosts;
        if (Order.ReturnToOrigin)
        {
            startCosts *= 2;
        }
        bool NoSpacePointsAvailableForStartAndOrder = SpacePointsAvailable < SpacePointsOrdered + startCosts;
        // �berpr�fe, ob genug SpacePoints vorhanden sind f�r Startkosten und Order (Vollst�ndig oder nicht)
        if (Order.OnlyFullShipment && NoSpacePointsAvailableForStartAndOrder)
        {
            UnityEngine.Debug.Log("NoSpacePointsAvailableForStartAndOrder:" + NoSpacePointsAvailableForStartAndOrder);
            return false; // Wenn Order vollst�ndig ausgef�hrt werden soll und zu wenig SpacePoints f�r Order und Start vorhanden sind, Order kann nicht erf�llt werden
        }
        else if (SpacePointsAvailable < SpaceShip.SpaceShipStartSpacePointsCosts)
        {
            return false; // Nicht genug SpacePoints zum Start vorhanden sind, Order kann nicht erf�llt werden
        }
        else if (!Order.OnlyFullShipment)
        {
            return true; // Wenn die Order nicht vollst�ndig sein muss, ist sie immer erf�llbar, solange die Startkosten gedeckt sind
        }
        foreach (ResourceStorage shipmentDetail in Order.ResourceShipmentDetails)
        {
            // �berpr�fe, ob der ResourceType im Dictionary existiert
            if (!ResourceStorageCelestialBody.TryGetValue(shipmentDetail.ResourceType, out var storage))
            {
                return false; // ResourceType nicht gefunden, Order kann nicht erf�llt werden
            }
            // �berpr�fe, ob die Menge im Dictionary gr��er oder gleich der geforderten Menge ist
            if (storage.StorageQuantity < shipmentDetail.StorageQuantity)
            {
                return false; // Nicht genug Ressourcen, Order kann nicht erf�llt werden
            }
        }
        return true; // Alle Bedingungen erf�llt, Order kann erf�llt werden
    }


    public void CreateOrderOnCelestialBody(ResourceTransferOrder order)
    {
        Order = order;
        Order.Origin.AddResourceTransferOrder(Order);

    }

    public ResourceTransferOrder CreateOrderFromGui(String ResourceType, int ResourceAmount, CelestialBody origin, CelestialBody destination, int repetitions, bool isPrioritized, bool onlyFullShipment, bool ReturnToOrigin, bool IsForever)
    {
        return new ResourceTransferOrder(new List<ResourceStorage>() { new ResourceStorage(ResourceType, 0, ResourceAmount, 0, 0) }, origin, destination, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin, IsForever);
    }

}
