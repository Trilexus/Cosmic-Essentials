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
            DontDestroyOnLoad(gameObject); // Dies stellt sicher, dass der GameManager zwischen den Szenenwechseln nicht zerst�rt wird.
            CreateStructures();
            //Create Structure Database
            StructureDatabase.Initialize();
        }
        else
        {
            Destroy(gameObject); // Zerst�rt das zus�tzliche GameManager-Objekt, wenn es bereits ein aktives gibt.
        }
    }

    public void BuildStructure(StructureScriptableObject structureScriptableObject)
    {
        Debug.Log("Build Structure");
        Structure structureToBuild = structureDictionary[structureScriptableObject];
        Debug.Log("Structure to build: " + structureToBuild.Name);
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        Debug.Log("Celestial Body: " + celestialBody.name);
        if (structureToBuild != null)
        {
            celestialBody.InitiateConstructionStructure(structureToBuild);
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
}
