using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<Structure> GetStructuresForCelestialBody(AllowedLocation celestialBodyAllowedLocation)
    {
        return AllStructures.Where(I => I.AllowedLocations.Contains(celestialBodyAllowedLocation)).ToList();
    }

    public List<StructureScriptableObject> GetStructuresScriptableObjectForCelestialBody(AllowedLocation celestialBodyAllowedLocation)
    {
        return structureDictionary.Keys.Where(I => I.AllowedLocations.Contains(celestialBodyAllowedLocation)).ToList();
    }

    private void CreateStructures()
    {
        foreach (var scriptableObject in structureScriptableObjects)
        {
            Structure newStructure = new Structure(scriptableObject);
            structureDictionary[scriptableObject] = newStructure;
        }
    }
}
