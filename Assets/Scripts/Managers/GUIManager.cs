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
    public GameObject selectedPlanetarySystem;
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
        this.selectedCelestialBody = selectedCelestialBody;
        SetActivePlanetarySystem(selectedCelestialBody.transform.parent.gameObject);
        MoveCelestialBodyMenu(selectedCelestialBody);
        UpdateCelestialBodyInfos();
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
    public void ShowCelestialBodyInfos()
    {
        foreach (Transform child in CelestialBodyInfo.transform)
        {
            Destroy(child.gameObject);
        }
        Vector3 CelestialBodyInfoSymbolOffset = new Vector3(50, 0, 0);
        RectTransform InfoRect = CelestialBodyInfo.GetComponent<RectTransform>();
        Vector3 SymbolPosition = new Vector3(InfoRect.position.x, InfoRect.position.y,0);
        foreach (Area area in selectedCelestialBody.GetComponent<CelestialBody>().Areas)
        {
            GameObject Symbol = Instantiate(area.structure.Symbol);
            Symbol.transform.SetParent(CelestialBodyInfo.transform);
            Symbol.transform.position = SymbolPosition;
            Symbol.GetComponent<RectMask2D>().padding = new Vector4(45 - (0.45f * area.constructionProgress), 0, 0, 0);
            SymbolPosition = new Vector3(SymbolPosition.x + CelestialBodyInfoSymbolOffset.x, SymbolPosition.y, 0);
                    
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
