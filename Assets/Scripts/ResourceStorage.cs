using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceStorage
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public int MinQuantity = 0;
    [SerializeField]
    public int MaxQuantity;
    [SerializeField]
    public int BaseQuantity;
    [SerializeField]
    public int StorageQuantity;
    [SerializeField]
    public int ProductionQuantity;
    [SerializeField]
    public int ConsumptionQuantity;
    

    public ResourceStorage(string name, int maxQuantity, int storageQuantity, int productionQuantity, int consumptionQuantity)
    {
        this.Name = name;
        this.MaxQuantity = maxQuantity;
        this.StorageQuantity = storageQuantity;
        this.ProductionQuantity = productionQuantity;
        this.ConsumptionQuantity = consumptionQuantity;
    }

    public void CalculateResourceStorage()
    {
        StorageQuantity = Mathf.Clamp(StorageQuantity + ProductionQuantity + ConsumptionQuantity, MinQuantity, MaxQuantity);
    }

    public void ResetResourceProduction()
    {
        ProductionQuantity = 0;
        ConsumptionQuantity = 0;
    }
}

public enum ResourceType
{
    Food = 0,
    Metal = 1,
    Energy = 2,
    SpacePoints = 3
}