using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField]
    private ResourceStorage ResourceStorage;
    [SerializeField]
    CelestialBody target;
    [SerializeField]
    public bool isStarted = false;
    [SerializeField]
    public bool isArrived = false;
    [SerializeField]
    float speed = 0.01f;
    float rotationSpeed = 150f;
    
    




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
        }
    }

    public void StartJourney(CelestialBody target)
    {
        this.target = target;
        RotateToTarget();
        isStarted = true;
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
