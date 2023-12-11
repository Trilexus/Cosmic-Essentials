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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStructure()
    {
        ClearMenu();
        CreateMenuForCelestialBody(GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>().AllowedLocation);
        CreateMenuForCelestialBody();
    }

    public void CreateMenuForCelestialBody(LocationType Location)
    {
        ResearchManager.Instance.OnResearchNodeBuildableEntityDone += UpdateStructure;
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




    public void ClearMenu()
    {
        ResearchManager.Instance.OnResearchNodeBuildableEntityDone -= UpdateStructure;
        foreach (Transform entry in structureMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
        foreach (Transform entry in spaceFleetMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
    }
}
