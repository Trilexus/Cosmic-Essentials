using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip_TransporterMisslePool : MonoBehaviour
{
    public static SpaceShip_TransporterMisslePool Instance;
    public GameObject SpaceShip_TransporterMisslePrefab;
    public int amountToPool;

    private List<GameObject> pooledSpaceShip_TransporterMissle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledSpaceShip_TransporterMissle = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            CreateNewObjectInPool();
        }
    }

    private SpaceShip CreateNewObjectInPool()
    {
        GameObject SpaceShip_TransporterMissle = Instantiate(SpaceShip_TransporterMisslePrefab);
        SpaceShip_TransporterMissle.SetActive(false);
        pooledSpaceShip_TransporterMissle.Add(SpaceShip_TransporterMissle);        
        SpaceShip ship = SpaceShip_TransporterMissle.GetComponent<SpaceShip>();
        ship.ResetResources();
        return ship;
    }

    public SpaceShip GetPooledSpaceShip()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledSpaceShip_TransporterMissle[i].activeInHierarchy)
            {
                SpaceShip ship = pooledSpaceShip_TransporterMissle[i].GetComponent<SpaceShip>();
                ship.ResetResources();
                return ship;
            } 
        }
        return CreateNewObjectInPool();
    }

    public void ReturnSpaceShipToPool(GameObject spaceShip)
    {
        spaceShip.SetActive(false);
    }
}