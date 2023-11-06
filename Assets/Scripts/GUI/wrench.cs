using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wrench : MonoBehaviour
{

    float rotationSpeed = -100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate (Vector3.forward * Time.deltaTime * rotationSpeed);
        if (transform.rotation.eulerAngles.z < -45f)
        {
            rotationSpeed = 100f;
        } else if (transform.rotation.eulerAngles.z > 45f)
        {
           rotationSpeed = -100f;
        }
    }
}
