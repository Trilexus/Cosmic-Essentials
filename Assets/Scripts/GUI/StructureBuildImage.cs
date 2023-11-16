using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StructureBuildImage : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject parentGameObject = transform.parent.gameObject;
        //StructureMenuEntry parentScript = parentGameObject.GetComponent<StructureMenuEntry>();
        MenuEntry parentScript = parentGameObject.GetComponent<MenuEntry>();
        if (parentScript is StructureMenuEntry structureScript)
        {
            BuildingInterfaceManager.Instance.BuildStructure(structureScript.structureData);            
        }else if (parentScript is SpacefleetMenuEntry spacefleetScript)
        {
            BuildingInterfaceManager.Instance.BuildSpacefleet(spacefleetScript.spaceFleetData);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
