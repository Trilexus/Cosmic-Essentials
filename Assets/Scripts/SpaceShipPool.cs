using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceShipPool : MonoBehaviour
{
    public static SpaceShipPool Instance;
    public int amountToPool;

    private List<GameObject> pooledSpaceShip;
    private Dictionary<SpacefleetScriptableObject, List<GameObject>> SpaceFleetShips = new Dictionary<SpacefleetScriptableObject, List<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
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
                SpaceShip ship = spaceship.GetComponent<SpaceShip>();
                ship.InitializedSpaceShip(spacefleetScriptableObject);
                return ship;
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
        SpaceShip ship = spaceShip.GetComponent<SpaceShip>();
        ship.InitializedSpaceShip(spacefleetScriptableObject);
        return ship;
    }

    public void ReturnSpaceShipToPool(GameObject spaceShip)
    {
        spaceShip.SetActive(false);
    }
}
