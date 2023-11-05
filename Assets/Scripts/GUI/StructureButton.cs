using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureButton : MonoBehaviour
{
    public StructureScriptableObject structureData; // Dein ScriptableObject
    public string structureName;

    public void Build()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        if (structureData != null)
        {
            BuildingInterfaceManager.Instance.BuildStructure(structureData);
        } else if (structureName.Equals("SpaceStation"))
        {
            BuildingInterfaceManager.Instance.BuildSpaceStation();
        }else
        {
            Debug.Log("Build: nichts");
        }
    }
    public void Demolish()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        BuildingInterfaceManager.Instance.DemolishStructure(structureData);
    }
}
