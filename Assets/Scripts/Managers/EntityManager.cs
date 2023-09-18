using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// EntityManager stores Structure prefabs.
public class EntityManager : MonoBehaviour
{
    public List<StructureScriptableObject> structureScriptableObjects;
    [SerializeField]
    public List<Structure> AllStructures = new List<Structure>();
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateStructures()
    {
        foreach (var scriptableObject in structureScriptableObjects)
        {
            Structure newStructure = new Structure(scriptableObject);
            AllStructures.Add(newStructure);
        }
    }
}
