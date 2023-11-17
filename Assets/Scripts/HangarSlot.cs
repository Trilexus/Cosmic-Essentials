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
    public delegate void ShipConstructionStatusHandler(HangarSlot slot);
    public event ShipConstructionStatusHandler OnShipConstructionStatusChanged;


    public void AddTooConstructionStatus(int value)
    {
        constructionProgress = Mathf.Clamp(constructionProgress + value, 0, 100);
        constructionProgress = Math.Clamp(constructionProgress, 0, 100);
         // Benachrichtige die Abonnenten über die Statusänderung
         OnShipConstructionStatusChanged?.Invoke(this);
    }

    public HangarSlot(SpacefleetScriptableObject spacefleetScriptableObject, int consturctionProgress)
    {
        this.spacefleetScriptableObject = spacefleetScriptableObject;
        this.constructionProgress = consturctionProgress;
    }

}
