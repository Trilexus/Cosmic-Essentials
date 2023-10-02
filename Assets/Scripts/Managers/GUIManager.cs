using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;
    public GameObject CelestialBodyMenu;
    public GameObject CelestialBodyInfo;

    public Vector3 celestialBodyMenuOffset;
    public Vector3 celestialBodyInfoOffset;

    public GameObject selectedCelestialBody;
    public GameObject selectedPlanetarySystem;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Singleton
            DontDestroyOnLoad(this.gameObject); // Singleton
            
        }
        else
        {
            Destroy(this.gameObject); // Singleton
        }
    }

    public void SetActiveCelestialBody(GameObject selectedCelestialBody)
    {
        this.selectedCelestialBody = selectedCelestialBody;
        SetActivePlanetarySystem(selectedCelestialBody.transform.parent.gameObject);
        MoveCelestialBodyMenu(selectedCelestialBody);
        ShowCelestialBodyInfos();
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
        ShowCelestialBodyMenu();
    }
    
    public void ShowCelestialBodyInfos()
    {
        Vector3 CelestialBodyInfoSymbolOffset = new Vector3(35, -35, 0);
        RectTransform InfoRect = CelestialBodyInfo.GetComponent<RectTransform>();
        Vector3 SymbolPosition = new Vector3(InfoRect.position.x + CelestialBodyInfoSymbolOffset.x, InfoRect.position.y + CelestialBodyInfoSymbolOffset.y,0);
        foreach (Area area in selectedCelestialBody.GetComponent<CelestialBody>().Areas)
        {

            GameObject Symbol = Instantiate(area.structure.Symbol);
            Symbol.transform.SetParent(CelestialBodyInfo.transform);
            Symbol.transform.position = SymbolPosition;
                    
        }
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
