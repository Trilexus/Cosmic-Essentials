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

    private GameObject CreateNewObjectInPool()
    {
        GameObject SpaceShip_TransporterMissle = Instantiate(SpaceShip_TransporterMisslePrefab);
        SpaceShip_TransporterMissle.SetActive(false);
        pooledSpaceShip_TransporterMissle.Add(SpaceShip_TransporterMissle);
        return SpaceShip_TransporterMissle;
    }

    public GameObject GetPooledLine()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledSpaceShip_TransporterMissle[i].activeInHierarchy)
            {
                return pooledSpaceShip_TransporterMissle[i];
            } 
        }
        return CreateNewObjectInPool();
    }

    public void ReturnLineToPool(GameObject line)
    {
        line.SetActive(false);
    }
}