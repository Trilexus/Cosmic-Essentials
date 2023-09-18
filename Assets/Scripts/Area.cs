using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Area
{
    [SerializeField]
    public Structure structure;
    [SerializeField]
    public int constructionProgress; // 0 -> 100 percent 

}
