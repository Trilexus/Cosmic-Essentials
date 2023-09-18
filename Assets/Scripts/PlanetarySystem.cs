using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetarySystem : MonoBehaviour
{
    public string Name { get; private set; }
    [SerializeField]
    public List<CelestialBody> celestialBodies = new List<CelestialBody>();

    public List<GameObject> neighbors;
    public List<Resources> ResourceStorage;



    // Start is called before the first frame update
    void Start()
    {
        CollectAllCelestialBodys();
    }

    private void CollectAllCelestialBodys()
    {
        foreach (Transform child in transform)
        {
            CelestialBody celestialBody = child.GetComponent<CelestialBody>();
            if (celestialBody != null)
            {
                celestialBodies.Add(celestialBody);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
