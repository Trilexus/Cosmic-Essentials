using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterfaceManager : MonoBehaviour
{
    public static BuildingInterfaceManager Instance;

    //EntityManager.Instance.structureDictionary;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Singleton
            DontDestroyOnLoad(this.gameObject); // Singleton
        }
        else
        {
            Destroy(this.gameObject); // Singleton
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildStructure(StructureScriptableObject structureScriptableObject)
    {
        Structure structureToBuild = EntityManager.Instance.structureDictionary[structureScriptableObject];

        //CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        if (GUIManager.Instance?.selectedCelestialBody?.GetComponent<CelestialBody>() is CelestialBody celestialBody)
        {
            bool areResourcesSufficient = structureToBuild.AreResourcesSufficientForStructure(celestialBody);
            bool isLocationAllowed = structureToBuild.IsLocationAllowed(celestialBody.AllowedLocationType);
            if (!isLocationAllowed)
            {
                //Debug.Log("Location not allowed to build " + structureToBuild.Name);
                GUIManager.Instance.MentatScript.SetAlertText("StructureNotAllowed");
                return;
            }
            else if (!areResourcesSufficient)
            {
                //Debug.Log("Not enough resources to build " + structureToBuild.Name);
                GUIManager.Instance.MentatScript.SetAlertText("InsufficientResources");
                return;
            }
            foreach (var resource in structureToBuild.Costs)
            {
                celestialBody.ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity -= resource.Quantity;
            }
            if (structureToBuild != null)
            {
                celestialBody.InitiateConstructionStructure(structureToBuild);
            }
        }
    }

    public void BuildSpaceStation()
    {
        PlanetarySystem planetarySystem = GUIManager.Instance.selectedPlanetarySystem.GetComponent<PlanetarySystem>();
        planetarySystem.BuildSpaceStation();
    }

    public void DemolishStructure(StructureScriptableObject structureScriptableObject)
    {

        Structure structureToDemolish = EntityManager.Instance.structureDictionary[structureScriptableObject];
        //CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        if (GUIManager.Instance?.selectedCelestialBody?.GetComponent<CelestialBody>() is CelestialBody celestialBody)
        {
            if (structureToDemolish != null)
            {
                celestialBody.InitiateDemolishStructure(structureToDemolish);
            }
        }
    }
}
