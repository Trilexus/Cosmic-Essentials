using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// EntityManager stores Structure prefabs.
public class EntityManager : MonoBehaviour
{
    public List<StructureScriptableObject> structureScriptableObjects;
    [SerializeField]
    public List<Structure> AllStructures = new List<Structure>();
    public Dictionary<StructureScriptableObject, Structure> structureDictionary = new Dictionary<StructureScriptableObject, Structure>();

    public static EntityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Dies stellt sicher, dass der GameManager zwischen den Szenenwechseln nicht zerstört wird.
            CreateStructures();
            //Create Structure Database
            StructureDatabase.Initialize();
        }
        else
        {
            Destroy(gameObject); // Zerstört das zusätzliche GameManager-Objekt, wenn es bereits ein aktives gibt.
        }
    }

    public void BuildStructure(StructureScriptableObject structureScriptableObject)
    {
        Structure structureToBuild = structureDictionary[structureScriptableObject];
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        if (structureToBuild != null)
        {
            celestialBody.InitiateConstructionStructure(structureToBuild);
        }
    }

    public void DemolishStructure(StructureScriptableObject structureScriptableObject)
    {
        Structure structureToDemolish = structureDictionary[structureScriptableObject];
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        if (structureToDemolish != null)
        {
            celestialBody.InitiateDemolishStructure(structureToDemolish);
        }
    }

    private void CreateStructures()
    {
        foreach (var scriptableObject in structureScriptableObjects)
        {
            Structure newStructure = new Structure(scriptableObject);
            structureDictionary[scriptableObject] = newStructure;
        }
    }

    public void BuildSpaceStation()
    {
        PlanetarySystem planetarySystem = GUIManager.Instance.selectedPlanetarySystem.GetComponent<PlanetarySystem>();
        planetarySystem.BuildSpaceStation();
    }
}
