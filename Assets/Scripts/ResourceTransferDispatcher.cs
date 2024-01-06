using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceTransferDispatcher : MonoBehaviour
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

    
    public void LoadSpaceShip(SpaceShip spaceShip)
    {
        foreach (var orderResource in Order.ResourceShipmentDetails)
        {
            ResourceType currentType = orderResource.ResourceType; // Typ der Order holen.
            int orderStorage = orderResource.StorageQuantity; // Menge der Order holen.
            int celestialBodyStorage = ResourceStorageCelestialBody[currentType].StorageQuantity; // Menge der Ressource auf dem Planeten holen.
            int AvailableResourceQuantity = Math.Min(orderStorage, celestialBodyStorage); // Minimale Menge der Ressource berechnen.
            ResourceStorageCelestialBody[orderResource.ResourceType].StorageQuantity -= AvailableResourceQuantity; // Ressourcen auf dem Planeten reduzieren.
            spaceShip.FreeSpace -= AvailableResourceQuantity; // Lagerkapazit�t des SpaceShips reduzieren.
            spaceShip.ResourceStorage[orderResource.ResourceType].StorageQuantity += AvailableResourceQuantity; // Ressourcen im SpaceShip erh�hen.
            if (spaceShip.FreeSpace <= 0) // SpaceShip voll, dann abbrechen.
            {
                break;
            }
        }
    }
    public void LoadSpaceShip(SpaceShip spaceShip, Dictionary<ResourceType, ResourceStorage> ResourceStorage)
    {
        spaceShip.ResourceStorage = ResourceStorage; 
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
    public ResourceTransferOrder CreateOrderFromGui(String ResourceType, int ResourceAmount, CelestialBody origin, CelestialBody destination, int repetitions, bool isPrioritized, bool onlyFullShipment, bool ReturnToOrigin, bool IsForever, SpacefleetScriptableObject spacefleetScriptableObject)
    {
        return new ResourceTransferOrder(new List<ResourceStorage>() { new ResourceStorage(ResourceType, 0, ResourceAmount, 0, 0) }, origin, destination, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin, IsForever, spacefleetScriptableObject);
    }


    //------------------------------------------------------------------------------------

    //Einfache Pr�fung, ob die Ressourcen f�r eine Order vorhanden sind.
    public bool CheckResourcesForOrder(ResourceTransferOrder order)
    {
        Debug.Log(order == null);
        foreach (ResourceStorage shipmentDetail in order.ResourceShipmentDetails)
        {
            // �berpr�fe, ob der ResourceType im Dictionary existiert
            if (!ResourceStorageCelestialBody.TryGetValue(shipmentDetail.ResourceType, out var storage))
            {
                return false; // ResourceType nicht gefunden, Order kann nicht erf�llt werden
            }
            // �berpr�fe, ob die Menge im Dictionary gr��er oder gleich der geforderten Menge ist
            if (storage.StorageQuantity < shipmentDetail.StorageQuantity && order.OnlyFullShipment)
            {
                return false; // Nicht genug Ressourcen, Order kann nicht erf�llt werden
            }
            else if (storage.StorageQuantity == 0)
            {
                return false;
            }
        }
        return true;
    }

    public SpaceShip SelectSpaceShip(ResourceTransferOrder order, CelestialBody celestialBody)
    {
        if (order.AutoChooseShip)
        {
            Debug.Log("GetSpaceShip AutoChooseShip true");
            return celestialBody.PerformHangarOperation(hangar => hangar.GetSpaceShipForOrderAmount(order.ResourceShipmentDetails[0].StorageQuantity));
        }else {
            Debug.Log("GetSpaceShip AutoChooseShip false");
            if (celestialBody.PerformHangarOperation(hangar => hangar.GetSpaceFleetCount(order.SpacefleetScriptableObject,true, true) > 0))
            {
                Debug.Log("GetSpaceShip Schiff vorhanden");
                return celestialBody.PerformHangarOperation(hangar => hangar.GetSpaceShip(order.SpacefleetScriptableObject, false, true));
            }
            else
            {
                Debug.Log("GetSpaceShip Schiff nicht vorhanden");
                return null;
            }
        }
    }


    public void DispatchOrders()
    {
        foreach (var order in ResourceTransferOrders) // Alle Orders durchgehen.
        {
            Order = order;
            bool canFulfillOrder = CheckResourcesForOrder(order); // �berpr�fen, ob die Order erf�llt werden kann.
            if (canFulfillOrder)
            {
                //Debug.Log("Order kann erf�llt werden");
                //Debug.Log("Order AutoChooseShip: " + order.AutoChooseShip);
                SpaceShip AvailableShip = SelectSpaceShip(order, CelestialBody);
                if (AvailableShip != null) { 
                    LoadSpaceShip(AvailableShip); // SpaceShip beladen.
                    if (!order.IsForever)
                    {
                        order.Repetitions--; // Anzahl der Wiederholungen reduzieren.
                    }
                    AvailableShip.StartJourneyOrder(order.Origin, order.Target, order.ReturnToOrigin); // SpaceShip starten.
                    break; // Order wurde erf�llt, deswegen wird die Schleife abgebrochen.
                }
            }
            else if (order.IsPrioritized)
            {
                break; // Order kann nicht erf�llt werden, ist aber priorisiert, deswegen wird die Schleife abgebrochen.
            }
        }
        if (Order.Repetitions == 0) // Order wurde erf�llt, deswegen wird die Order aus der Liste entfernt.
        {
            ResourceTransferOrders.Remove(Order);
            Order.Repetitions--; // Anzahl der Wiederholungen reduzieren, damit sie auch aus der GUI verschwindet.
        }
        else if (!Order.IsPrioritized) // Order an das Ende der Liste anh�ngen, wenn sie nicht priorisiert ist.
        {
            ResourceTransferOrders.Remove(Order); // Order aus der Liste entfernen.
            ResourceTransferOrders.Add(Order); // Order wieder hinten an die Liste anh�ngen.
        }
    }
}
