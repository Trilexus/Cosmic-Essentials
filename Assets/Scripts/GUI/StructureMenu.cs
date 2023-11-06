using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructureMenu : MonoBehaviour
{
    [SerializeField]
    GameObject structureMenuEntryPrefab;
    [SerializeField]
    GameObject structureMenuContent;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMenuForCelestialBody(AllowedLocation allowedLocation)
    {
        EntityManager.Instance.GetStructuresScriptableObjectForCelestialBody(allowedLocation).ForEach(I =>
        {
            GameObject newStructureMenuEntry = Instantiate(structureMenuEntryPrefab, structureMenuContent.transform);
            newStructureMenuEntry.transform.SetParent(structureMenuContent.transform);
            StructureMenuEntry structureMenuEntry = newStructureMenuEntry.GetComponent<StructureMenuEntry>();

            structureMenuEntry.structureData = I;
            structureMenuEntry.GetAndSetStructureInfoTextAndImage();
        });
    }

    public void ClearMenu()
    {
        foreach (Transform entry in structureMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
    }
}
