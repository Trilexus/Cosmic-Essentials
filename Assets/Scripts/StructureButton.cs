using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureButton : MonoBehaviour
{
    public StructureScriptableObject structureData; // Dein ScriptableObject

    public void OnButtonClick()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        Debug.Log("Button Clicked");
        EntityManager.Instance.BuildStructure(structureData);
    }
}
