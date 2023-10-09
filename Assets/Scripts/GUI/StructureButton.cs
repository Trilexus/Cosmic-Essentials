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
            
            Debug.Log("Build: " + structureData);
            EntityManager.Instance.BuildStructure(structureData);
        } else if (structureName.Equals("SpaceStation"))
        {
            Debug.Log("Build: " + structureName);
            EntityManager.Instance.BuildSpaceStation();
        }else
        {
            Debug.Log("Build: nichts");
        }
    }
    public void Demolish()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        EntityManager.Instance.DemolishStructure(structureData);
    }
}
