using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HangarSlot
{
    [SerializeField]
    public SpacefleetScriptableObject spacefleetScriptableObject;
    [SerializeField]
    public int constructionProgress; // 0 -> 100 Prozent


}
