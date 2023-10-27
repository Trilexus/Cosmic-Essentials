using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField]
    public Dictionary<ResourceType, ResourceStorage> ResourceStorageSpaceShip = new Dictionary<ResourceType, ResourceStorage>();
    public int maxStorage = 100;
    public int FreeStorageSpace = 100;

    [SerializeField]
    public CelestialBody target;
    [SerializeField]
    public CelestialBody origin;
    public bool FliesBack = false;
    [SerializeField]
    public bool isStarted = false;
    [SerializeField]
    public bool isArrived = false;
    [SerializeField]
    float isArrivedDistance = 0.001f;
    [SerializeField]
    float speed = 0.01f;
    float rotationSpeed = 150f;
    public static int SpaceShipStartSpacePointsCosts = 100;
    [SerializeField]
    public static Dictionary<ResourceType, ResourceStorage> SpaceShipCosts = new Dictionary<ResourceType, ResourceStorage> {
        { ResourceType.Metal, new ResourceStorage(ResourceType.Metal, 100, 0, 0, 0) },
        { ResourceType.Energy, new ResourceStorage(ResourceType.Energy, 100, 0, 0, 0) },
    };


    public void ResetResources()
    {
        ResourceStorageSpaceShip.Clear();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceStorageSpaceShip[type] = new ResourceStorage(type.ToString(), 100, 0, 0, 0);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        RotateToTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
            if (Vector2.Distance(transform.position, target.transform.position) < isArrivedDistance)
            {
                isArrived = true;
                isStarted = false;
                target.RegisterTheSpaceship(this);
            }
        }
    }

    public void StartJourney(CelestialBody origin, CelestialBody target, bool fliesBack)
    {
        this.target = target;
        this.origin = origin;
        this.transform.position = origin.transform.position;
        RotateToTarget();
        gameObject.SetActive(true);
        isArrived = false;
        isStarted = true;
        FliesBack = fliesBack;
    }

    private void RotateToTargetOverTime()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Sanfte Rotation zum Ziel
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }
    private void RotateToTarget()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = targetRotation;
    }
}