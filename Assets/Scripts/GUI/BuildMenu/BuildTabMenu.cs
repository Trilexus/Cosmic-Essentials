using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildTabMenu : MonoBehaviour
{
    [SerializeField]
    GameObject structureEntryPrefab;
    [SerializeField]
    GameObject structureMenuContent;
    [SerializeField]
    GameObject spaceFleetEntryPrefab;
    [SerializeField]
    GameObject spaceFleetMenuContent;
    [SerializeField]
    GameObject InfoEntryPrefab;
    [SerializeField]
    GameObject InfoMenuContent;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMenuForCelestialBody(LocationType Location)
    {
        EntityManager.Instance.GetStructuresScriptableObjectForCelestialBody(Location).ForEach(I =>
        {
            GameObject newStructureMenuEntry = Instantiate(structureEntryPrefab, structureMenuContent.transform);
            newStructureMenuEntry.transform.SetParent(structureMenuContent.transform);
            StructureMenuEntry structureMenuEntry = newStructureMenuEntry.GetComponent<StructureMenuEntry>();

            structureMenuEntry.structureData = I;
            structureMenuEntry.GetAndSetStructureInfoTextAndImage();
        });
    }
    public void CreateMenuForCelestialBody()
    {
        EntityManager.Instance.GetSpacefleetScriptableObjectForCelestialBody().ForEach(I =>
        {
            GameObject newSpacefleetMenuEntry = Instantiate(spaceFleetEntryPrefab, spaceFleetMenuContent.transform);
            newSpacefleetMenuEntry.transform.SetParent(spaceFleetMenuContent.transform);
            SpacefleetMenuEntry SpacefleeMenuEntry = newSpacefleetMenuEntry.GetComponent<SpacefleetMenuEntry>();

            SpacefleeMenuEntry.spaceFleetData = I;
            SpacefleeMenuEntry.GetAndSetSpaceFleetInfoTextAndImage();
        });
    }


    public void CreateMenuForCelestialBody(CelestialBody celestialBody)
    {
        celestialBody.modifiers.ForEach(modifier =>
        {
            GameObject newInfoMenuEntry = Instantiate(InfoEntryPrefab, InfoMenuContent.transform);
            newInfoMenuEntry.transform.SetParent(InfoMenuContent.transform);
            newInfoMenuEntry.GetComponent<BuffEntry>().Initialize(modifier as ModifierResourceProductionFactor);
        });
    }



    public void ClearMenu()
    {
        foreach (Transform entry in structureMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
        foreach (Transform entry in spaceFleetMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
        foreach (Transform entry in InfoMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
    }
}
