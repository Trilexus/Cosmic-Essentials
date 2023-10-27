using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;
    public GameObject CelestialBodyMenu;
    public GameObject CelestialBodyInfo;
    public CelestialBodyInfo CelestialBodyInfoScript;

    public Vector3 celestialBodyMenuOffset;
    public Vector3 celestialBodyInfoOffset;

    public GameObject selectedCelestialBody;
    public GameObject selectedCelestialBodyTarget;
    public GameObject selectedPlanetarySystem;
    public GameObject ActiveCelestialBodyMarker; // shows the selected celestial body
    public GameObject ActiveCelestialBodyTarget; // shows the celestial body target for orders

    [SerializeField]
    SpriteRenderer selectedCelestialBodySprite;
    [SerializeField]
    SpriteRenderer selectedCelestialBodyTargetSprite;


    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Singleton
            DontDestroyOnLoad(this.gameObject); // Singleton
            CelestialBodyInfoScript = CelestialBodyInfo.GetComponent<CelestialBodyInfo>();
        }
        else
        {
            Destroy(this.gameObject); // Singleton
        }
    }

    public void SetActiveCelestialBody(GameObject selectedCelestialBody)
    {
        if (this.selectedCelestialBody == null)
        {
            this.selectedCelestialBody = selectedCelestialBody;
            selectedCelestialBodySprite = selectedCelestialBody.GetComponent<SpriteRenderer>();
            ActiveCelestialBodyMarker.transform.position = selectedCelestialBody.transform.position;
            ActiveCelestialBodyMarker.SetActive(true);
            SetActivePlanetarySystem(selectedCelestialBody.transform.parent.gameObject);
            //MoveCelestialBodyMenu(selectedCelestialBody);
            UpdateCelestialBodyInfos();
        } else if (this.selectedCelestialBody == selectedCelestialBody) {
            this.selectedCelestialBody = null;
            ActiveCelestialBodyMarker.SetActive(false);
            ActiveCelestialBodyTarget.SetActive(false);
        } else
        {
            selectedCelestialBodyTarget = selectedCelestialBody;
            selectedCelestialBodyTargetSprite = selectedCelestialBodyTarget.GetComponent<SpriteRenderer>();
            ActiveCelestialBodyTarget.transform.position = selectedCelestialBody.transform.position;
            ActiveCelestialBodyTarget.SetActive(true);
        }
    }

    private void SetActivePlanetarySystem(GameObject selectedPlanetarySystem)
    {
        if (selectedPlanetarySystem != this.selectedPlanetarySystem)
        {
            DeactivateCurrentPlanetarySystem();
            this.selectedPlanetarySystem = selectedPlanetarySystem;
            selectedPlanetarySystem.GetComponent<PlanetarySystem>().IsActivePlanetarySystem = true;
        }        
    }

    private void DeactivateCurrentPlanetarySystem()
    {
        if (selectedPlanetarySystem != null)
        {
            selectedPlanetarySystem.GetComponent<PlanetarySystem>().IsActivePlanetarySystem = false;
        }
    }

    public void MoveCelestialBodyMenu(GameObject selectedCelestialBody)
    {
        RectTransform rect = CelestialBodyMenu.GetComponent<RectTransform>();
        celestialBodyMenuOffset = new Vector3(rect.rect.width / 2, rect.rect.height / -2, 0);
        Vector3 newPosition = selectedCelestialBody.transform.position;
        newPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, newPosition);
        Vector3 newPositionWithOffset = newPosition + celestialBodyMenuOffset;
        rect.transform.position = newPositionWithOffset;
        CelestialBodyMenu.SetActive(true);
        UpdateCelestialBodyInfos();
    }
    
    public void UpdateCelestialBodyInfos()
    {
        CelestialBodyInfoScript.UpdateCelestialBodyInfos();
    }

    public void HideCelestialBodyMenu()
    {
        CelestialBodyMenu.SetActive(false);
    }
    public void ShowCelestialBodyMenu()
    {
        CelestialBodyMenu.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
