using System;
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
    private Dictionary<GameObject, CelestialBody> activeLines = new Dictionary<GameObject, CelestialBody>();
    [SerializeField]
    private GameObject planetarySystemCenterStar;
    private bool isActivePlanetarySystem = false;
    public GameObject SpaceStationPrefab;
    public bool IsActivePlanetarySystem
    {
        get { return isActivePlanetarySystem; }
        set
        {
            if (value == false)
            {
                DeactivateAffiliationLines();
            }
            else
            {
                isActivePlanetarySystem = value;
                ActivateAffiliationLines();
                Debug.Log("IsActive: " + isActivePlanetarySystem);
            }
        }
    }



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

    public void ActivateAffiliationLines()
    {
        foreach (CelestialBody celestialBody in celestialBodies.Where(i => i.isActiveAndEnabled))
        {
            GameObject LineRenderer = LinePool.Instance.GetPooledLine();
            LineRenderer.GetComponent<LineRenderer>().SetPosition(0, celestialBody.transform.position);
            LineRenderer.GetComponent<LineRenderer>().SetPosition(1, planetarySystemCenterStar.transform.position);
            LineRenderer.SetActive(true);
            activeLines.Add(LineRenderer, celestialBody);
        }
    }

    private void UpdateAffiliationLines()
    {
        foreach (KeyValuePair<GameObject, CelestialBody> entry in activeLines)
        {
            GameObject line = entry.Key;
            CelestialBody celestialBody = entry.Value;
            line.GetComponent<LineRenderer>().SetPosition(0, celestialBody.transform.position);
            line.GetComponent<LineRenderer>().SetPosition(1, planetarySystemCenterStar.transform.position);
        }
    }

    private void DeactivateAffiliationLines()
    {
        foreach (KeyValuePair<GameObject, CelestialBody> entry in activeLines)
        {
            GameObject line = entry.Key;
            line.SetActive(false);
        }
        activeLines.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivePlanetarySystem)
        {
            UpdateAffiliationLines();
        }
    }

    public void BuildSpaceStation()
    {
        SpaceStation spacestation = celestialBodies.FirstOrDefault(cb => cb.name == "SpaceStation") as SpaceStation;
        if (spacestation != null && !spacestation.isActiveAndEnabled)
        {
            Transform spacestationTransform = spacestation.gameObject.transform;
            // Jetzt können Sie mit spacestationTransform arbeiten
            spacestationTransform.gameObject.SetActive(true);
            spacestation.setPositionNearCelestialObject(GUIManager.Instance.selectedCelestialBody);
            spacestationTransform.SetParent(GUIManager.Instance.selectedCelestialBody.transform,false);            
            spacestation.StartBuilding();
        } else if (spacestation != null && spacestation.isActiveAndEnabled)
        {
            GUIManager.Instance.MentatScript.SetAlertText("Only one station per star system!");
        }else if (spacestation == null)
        {
            GameObject spaceStationGameObject = Instantiate(SpaceStationPrefab, GUIManager.Instance.selectedCelestialBody.transform.position, Quaternion.identity, this.transform);
            spacestation = spaceStationGameObject.GetComponent<SpaceStation>();
            spacestation.name = "SpaceStation";
            celestialBodies.Add(spacestation);
            spaceStationGameObject.transform.gameObject.SetActive(true);
            spacestation.setPositionNearCelestialObject(GUIManager.Instance.selectedCelestialBody);
            spacestation.StartBuilding();
        }
    }
}
