using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField]
    public Structure Structure { get; set; } 
    public bool IsBuilt()
    {
        return Structure.Type != StructureType.FreeArea;
    }

}
