using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangarManager
{
    public delegate void HangarChangeHandler(HangarSlot hangarSlot);
    public event HangarChangeHandler OnHangarSlotChanged;

    private List<HangarSlot> hangar = new List<HangarSlot>();

    public void AddSpaceship(HangarSlot hangarSlot)
    {
        hangar.Add(hangarSlot);
        OnHangarSlotChanged?.Invoke(hangarSlot);
        hangarSlot.OnShipConstructionStatusChanged += HandleShipConstructionStatusChange;

    }

    public void RemoveSpaceship(HangarSlot hangarSlot)
    {
        hangarSlot.OnShipConstructionStatusChanged -= HandleShipConstructionStatusChange;
        hangar.Remove(hangarSlot);
        OnHangarSlotChanged?.Invoke(hangarSlot);
    }

    public List<HangarSlot> GetHangarSlots()
    {
        return hangar;
    }
    public List<HangarSlot> GetHangarSlots(bool isCompleted)
    {
        switch (isCompleted)
        {
            case true:
                return hangar.Where(x => x.constructionProgress >= 100).ToList();
            case false:
                return hangar.Where(x => x.constructionProgress < 100).ToList();
        }
    }
    public List<HangarSlot> GetHangarSlots(SpacefleetScriptableObject spacefleetScriptableObject, bool isCompleted)
    {
        switch (isCompleted)
        {
            case true:
                return hangar.Where(x => x.spacefleetScriptableObject == spacefleetScriptableObject && x.constructionProgress >= 100).ToList();
            case false:
                return hangar.Where(x => x.spacefleetScriptableObject == spacefleetScriptableObject && x.constructionProgress < 100).ToList();
        }
    }

    public int GetSpaceFleetCount(SpacefleetScriptableObject spacefleetScriptableObject, bool isCompleted)
    {
        switch (isCompleted)
        {
            case true:
                return hangar.Count(x => x.spacefleetScriptableObject == spacefleetScriptableObject && x.constructionProgress >= 100);
            case false:
                return hangar.Count(x => x.spacefleetScriptableObject == spacefleetScriptableObject && x.constructionProgress < 100);
        }
    }
    public int GetSpaceFleetCount(bool isCompleted)
    {
        switch (isCompleted)
        {
            case true:
                return hangar.Count(x => x.constructionProgress >= 100);
            case false:
                return hangar.Count(x => x.constructionProgress < 100);
        }
    }

    private void HandleShipConstructionStatusChange(HangarSlot hangarSlot)
    {
        OnHangarSlotChanged?.Invoke(hangarSlot);
    }
}
