using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Area
{
    [SerializeField]
    public Structure structure;
    [SerializeField]
    private int constructionProgress; // 0 -> 100 percent
    public int ConstructionProgress
    {
        get
        {
            return constructionProgress;
        }
        set
        {
            constructionProgress = value;
            OnConstructionProgressChanged?.Invoke(this);
        }
    }

    public delegate void ConstructionProgressChangeHandler(Area area);
    public event ConstructionProgressChangeHandler OnConstructionProgressChanged;
}
