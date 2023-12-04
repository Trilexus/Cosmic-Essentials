using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// EntityManager stores Structure prefabs.
public class EntityManager : MonoBehaviour
{
    [SerializeField]
    private List<StructureScriptableObject> structureScriptableObjects;
    [SerializeField]
    public List<Structure> AllStructures = new List<Structure>();
    public Dictionary<StructureScriptableObject, Structure> structureDictionary = new Dictionary<StructureScriptableObject, Structure>();

    [SerializeField]
    private List<SpacefleetScriptableObject> spacefleetScriptableObjects;
    public Dictionary<SpacefleetScriptableObject, SpaceFleet> spacefleetDictionary = new Dictionary<SpacefleetScriptableObject, SpaceFleet>();
    public delegate void SpacefleetChangeHandler(List<SpacefleetScriptableObject> spacefleetScriptableObjects);
    public event SpacefleetChangeHandler OnSpacefleetChanged;

    public static EntityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Dies stellt sicher, dass der GameManager zwischen den Szenenwechseln nicht zerstört wird.
            CreateStructures();
            SubscribeToEvents();
            //Create Structure Database
            StructureDatabase.Initialize();
        }
        else
        {
            Destroy(gameObject); // Zerstört das zusätzliche GameManager-Objekt, wenn es bereits ein aktives gibt.
        }
    }
    public void Start()
    {
        OnSpacefleetChanged?.Invoke(spacefleetScriptableObjects);
    }

    public void SubscribeToEvents()
    {
        ResearchManager.Instance.OnResearchNodeStructureProductionDone += UpgradeStructures;
    }

    public List<Structure> GetStructuresForCelestialBody(LocationType celestialBodyAllowedLocation)
    {
        return AllStructures.Where(I => I.AllowedLocations.Contains(celestialBodyAllowedLocation)).ToList();
    }

    public List<StructureScriptableObject> GetStructuresScriptableObjectForCelestialBody(LocationType celestialBodyAllowedLocation)
    {
        return structureDictionary.Keys
            .Where(structure => structure.AllowedLocations.Contains(celestialBodyAllowedLocation))
            .Where(structure => structure.ResearchRequirements.All(ResearchManager.Instance.ResearchNodeScriptableObjectsDone.Contains))
            .ToList();
    }


    public List<SpacefleetScriptableObject> GetSpacefleetScriptableObjectForCelestialBody()
    {
        return spacefleetScriptableObjects;
    }

    public List<SpacefleetScriptableObject> GetAllSpacefleetScriptableObjectByTypes(SpacefleetType type)
    {
        return spacefleetScriptableObjects.Where(spaceFleet => spaceFleet.Type == type).ToList();
    }

    public Structure GetStructure(StructureScriptableObject structureScriptableObject)
    {
        return structureDictionary[structureScriptableObject];
    }
    //TODO Funktioniert erst,,wenn Spacefleet erstellt werden können.
    //public Structure GetSpacefleet(SpacefleetScriptableObject spacefleetScriptableObject)
    //{
        //return spacefleetDictionary[spacefleetScriptableObject];
    //}


    private void CreateStructures()
    {
        structureDictionary.Clear();
        foreach (var scriptableObject in structureScriptableObjects)
        {
            Structure newStructure = new Structure(scriptableObject);
            //StructureScriptableObject structureScriptableObjectCopy = Instantiate(scriptableObject);
            structureDictionary[scriptableObject] = newStructure;
        }
    }

    public void UpgradeStructures(StructureResourceUpgrade structureResourceUpgrade)
    {
        foreach (StructureScriptableObject updateScriptableObject in structureResourceUpgrade.structureScriptableObjects)
        {
            structureDictionary[updateScriptableObject].Upgrades.Add(structureResourceUpgrade);
            Debug.Log("Upgrade " + updateScriptableObject.Name + " with " + structureResourceUpgrade.ResourcesType);
        }
    }
    private void CreateSpacefleet()
    {
        spacefleetDictionary.Clear();
        foreach (var scriptableObject in spacefleetScriptableObjects)
        {
            //TODO Hier sollen Spacefleets erstellt werden.
            //Dabei muss der Pool beachtet und angepasst werden!
            //SpaceFleet newSpacefleet = new SpaceShip(scriptableObject);
            //spacefleetDictionary[scriptableObject] = newStructure;
        }
    }
}
