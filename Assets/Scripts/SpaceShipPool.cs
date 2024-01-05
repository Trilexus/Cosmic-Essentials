using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpaceShipPool : MonoBehaviour
{
    public static SpaceShipPool Instance;
    public int amountToPool;
    [SerializeField]
    private List<GameObject> pooledSpaceShip;
    private Dictionary<SpacefleetScriptableObject, List<GameObject>> SpaceFleetShips = new Dictionary<SpacefleetScriptableObject, List<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }
    public void DebugRessources(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        TextMeshProUGUI debugTXT = GUIManager.Instance.DebugText;
        debugTXT.text += "Pool:\n";
        foreach (var resource in spacefleetScriptableObject.ResourceStorage)
        {
            debugTXT.text += resource.Key + " - " + resource.Value.StorageQuantity + "\n";
        }
    }

    public SpaceShip GetPooledSpaceShip(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        if (!SpaceFleetShips.ContainsKey(spacefleetScriptableObject))
        {
            SpaceFleetShips.Add(spacefleetScriptableObject, new List<GameObject>());
        }
        foreach (GameObject spaceship in SpaceFleetShips[spacefleetScriptableObject])
        {
            if (!spaceship.activeInHierarchy)
            {
                SpaceShip shipScript = spaceship.GetComponent<SpaceShip>();
                shipScript.InitializedSpaceShip(spacefleetScriptableObject);
                return shipScript;
            }
        }
       return CreateNewObjectInPool(spacefleetScriptableObject, SpaceFleetShips[spacefleetScriptableObject]);
    }

    private SpaceShip CreateNewObjectInPool(SpacefleetScriptableObject spacefleetScriptableObject, List<GameObject> spaceShipList)
    {
        GameObject spaceShipPrefab = spacefleetScriptableObject.Prefab;
        GameObject spaceShip = Instantiate(spaceShipPrefab);
        spaceShip.SetActive(false);
        spaceShipList.Add(spaceShip);
        SpaceShip shipScript = spaceShip.GetComponent<SpaceShip>();
        shipScript.InitializedSpaceShip(spacefleetScriptableObject);
        return shipScript;
    }

    public void ReturnSpaceShipToPool(GameObject spaceShip)
    {
        spaceShip.GetComponent<SpaceShip>().SpacefleetScriptableObject = null;
        spaceShip.SetActive(false);

    }
}
