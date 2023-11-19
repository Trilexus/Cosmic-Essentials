using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangarManager
{
    public delegate void HangarChangeHandler(HangarSlot hangarSlot);
    public event HangarChangeHandler OnHangarSlotChanged;

    private List<HangarSlot> hangar = new List<HangarSlot>();

    public void AddHangarSlot(HangarSlot hangarSlot)
    {
        hangar.Add(hangarSlot);
        OnHangarSlotChanged?.Invoke(hangarSlot);
        hangarSlot.OnShipConstructionStatusChanged += HandleShipConstructionStatusChange;
    }

    public bool AddSpaceShip(SpaceShip spaceShip)
    {
        AddHangarSlot(new HangarSlot(spaceShip.SpacefleetScriptableObject, 100));
        return true;
    }

    public void ManageHangar()
    {
        Debug.Log("ManageHangar");
    }

    public bool RemoveHangarSlot(HangarSlot hangarSlot)
    {
        hangarSlot.OnShipConstructionStatusChanged -= HandleShipConstructionStatusChange;
        hangar.Remove(hangarSlot);
        OnHangarSlotChanged?.Invoke(hangarSlot);
        return true;
    }

    public bool RemoveSpaceShip(SpaceShip spaceShip)
    {
        RemoveHangarSlot(hangar.Where(x => x.spacefleetScriptableObject.Equals(spaceShip.SpacefleetScriptableObject)).First());
        return true;
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

    public SpacefleetScriptableObject GetSpacefleetForOrderAmount(int amount)
    {

        List<SpacefleetScriptableObject> spacefleetScriptableObjects = new List<SpacefleetScriptableObject>();  
        foreach ( HangarSlot slot in hangar.Where(x => x.constructionProgress >= 100))
        {
            if (!spacefleetScriptableObjects.Contains(slot.spacefleetScriptableObject) && slot.spacefleetScriptableObject.CargoSpace >= amount)
            {
                spacefleetScriptableObjects.Add(slot.spacefleetScriptableObject);
            }
        }
        if (spacefleetScriptableObjects.Count == 0)
        {
            return null;
        } else
        {
            return spacefleetScriptableObjects.OrderBy(x => x.CargoSpace).First();
        }        
    }

    public SpaceShip GetSpaceShipForOrderAmount(int amount)
    {

        List<SpacefleetScriptableObject> spacefleetScriptableObjects = new List<SpacefleetScriptableObject>();
        List<HangarSlot> hangarSlots = new List<HangarSlot>();
        foreach (HangarSlot slot in hangar.Where(x => x.constructionProgress >= 100))
        {
            if (!spacefleetScriptableObjects.Contains(slot.spacefleetScriptableObject) && slot.spacefleetScriptableObject.CargoSpace >= amount)
            {
                hangarSlots.Add(slot);
                spacefleetScriptableObjects.Add(slot.spacefleetScriptableObject);
            }
        }
        if (spacefleetScriptableObjects.Count == 0)
        {
            return null;
        }
        else
        {
            HangarSlot slot = hangarSlots.OrderBy(slot => slot.spacefleetScriptableObject.CargoSpace).First();
            SpacefleetScriptableObject AvailableShip = slot.spacefleetScriptableObject;
            SpaceShip spaceShip = SpaceShipPool.Instance.GetPooledSpaceShip(slot.spacefleetScriptableObject).GetComponent<SpaceShip>(); //SpaceShip aus dem Pool holen.
            spaceShip.InitializedSpaceShip(AvailableShip);
            RemoveHangarSlot(slot);

            return spaceShip;
        }
    }

    public SpaceShip GetSpaceShip(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        HangarSlot slot = hangar.Where(x => x.spacefleetScriptableObject == spacefleetScriptableObject && x.constructionProgress >= 100).First();
        if (slot == null)
        {
            return null;
        }
        SpacefleetScriptableObject AvailableShip = slot.spacefleetScriptableObject;
        SpaceShip spaceShip = SpaceShipPool.Instance.GetPooledSpaceShip(slot.spacefleetScriptableObject).GetComponent<SpaceShip>(); //SpaceShip aus dem Pool holen.
        spaceShip.InitializedSpaceShip(AvailableShip);
        RemoveHangarSlot(slot);
        return spaceShip;
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
