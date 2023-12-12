using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewResource", menuName = "ScriptableObjects/ResourceScriptableObject")]


public class ResourceScriptableObject : ScriptableObject
{
    [SerializeField]
    public string Name { get { return ResourceType.ToString(); } }
    [SerializeField]
    public int Quantity;
    [SerializeField]
    public ResourceType ResourceType;
}
