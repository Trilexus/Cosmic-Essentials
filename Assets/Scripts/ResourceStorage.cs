using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Quantity;
    [SerializeField]
    public int MaxQuantity;

    public ResourceStorage(string name, int quantity, int maxQuantity)
    {
        this.Name = name;
        this.Quantity = quantity;
        this.MaxQuantity = maxQuantity;
    }
}
