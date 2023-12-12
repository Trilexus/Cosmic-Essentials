using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingInterfaceManager : MonoBehaviour
{
    public static BuildingInterfaceManager Instance;
    public delegate void BuildStructureDelegate(CelestialBody celestialBody);
    public event BuildStructureDelegate OnBuildStructure;

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
            bool isAreaAvailable = celestialBody.Areas.Count < celestialBody.maxAreas;
            bool isbuildable = AreRequiredBuildingsPresent(structureScriptableObject, celestialBody);
            //bool areResourcesSufficient = structureToBuild.AreResourcesSufficientForStructure(celestialBody);
            bool areResourcesSufficient = AreResourcesSufficientForEntity(structureScriptableObject);
            bool isLocationAllowed = structureToBuild.IsLocationAllowed(celestialBody.AllowedLocation);
            if (!isAreaAvailable)
            {
                GUIManager.Instance.MentatScript.SetAlertText("NoAreaAvailable");
                return;
            }
            if (!isbuildable)
            {                
                GUIManager.Instance.MentatScript.SetAlertText("RequirementNotFulfilled");
                return;
            }else if (!isLocationAllowed)
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
            celestialBody.population.CurrentPopulation -= structureScriptableObject.CostsPopulation;
            if (structureToBuild != null)
            {
                celestialBody.InitiateConstructionStructure(structureToBuild, 0);
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

    internal void BuildSpacefleet(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        bool areRequiredBuildingsPresent = AreRequiredBuildingsPresent(spacefleetScriptableObject, celestialBody);
        bool areResourcesSufficient = AreResourcesSufficientForEntity(spacefleetScriptableObject);
        bool arePopulationRequirementsFulfilled = celestialBody.population.CurrentPopulation >= spacefleetScriptableObject.LivingSpace;
        if (spacefleetScriptableObject.Type == SpacefleetType.SpaceStation && areResourcesSufficient && areRequiredBuildingsPresent)
        {
            BuildSpaceStation();
        }
        else if (areResourcesSufficient && areRequiredBuildingsPresent && arePopulationRequirementsFulfilled)
        {
            foreach (ResourceScriptableObject resource in spacefleetScriptableObject.Costs)
            {
                celestialBody.ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity -= resource.Quantity;
            }
            celestialBody.population.CurrentPopulation -= spacefleetScriptableObject.LivingSpace;
            celestialBody.InitiateConstructionSpacefleet(spacefleetScriptableObject);

        } else if(!areRequiredBuildingsPresent)
        {
            GUIManager.Instance.MentatScript.SetAlertText("BuildingRequirementsNotFulfilled");
        } else if (!areResourcesSufficient || arePopulationRequirementsFulfilled)
        {
            GUIManager.Instance.MentatScript.SetAlertText("InsufficientResources");
        }
    }

    public bool AreResourcesSufficientForEntity(EntityScriptableObject entityScriptableObject)
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        foreach (ResourceScriptableObject resource in entityScriptableObject.Costs)
        {
            if (celestialBody.ResourceStorageCelestialBody[resource.ResourceType].StorageQuantity < resource.Quantity)
            {
                return false;
            }
        }
        return true;
    }

    public bool AreRequiredBuildingsPresent(EntityScriptableObject ScriptableObject, CelestialBody celestialBody)
    {
        Structure structure;
        List<StructureScriptableObject> structureRequirements = ScriptableObject.StructureRequirements;
        foreach (StructureScriptableObject structureRequirement in structureRequirements)
        {
            structure = EntityManager.Instance.GetStructure(structureRequirement);
            bool hatStrukturMitVollemFortschritt = celestialBody.Areas.Any(area => area.structure.Type == structure.Type && area.ConstructionProgress >= 100);
            if (!hatStrukturMitVollemFortschritt)
            {
                return false;
            }
        }
        return true;
    }
}
