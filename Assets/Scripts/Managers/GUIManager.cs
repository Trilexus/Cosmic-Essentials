using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;
    public GameObject CelestialBodyMenu;
    public Vector3 celestialBodyMenuOffset;
    public GameObject selectedCelestialBody;
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

    public void MoveCelestialBodyMenu(GameObject selectedCelestialBody)
    {
        this.selectedCelestialBody = selectedCelestialBody;
        RectTransform rect = CelestialBodyMenu.GetComponent<RectTransform>();
        celestialBodyMenuOffset = new Vector3(rect.rect.width / 2, rect.rect.height / -2, 0);
        Vector3 newPosition = selectedCelestialBody.transform.position;
        newPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, newPosition);
        Vector3 newPositionWithOffset = newPosition + celestialBodyMenuOffset;
        rect.transform.position = newPositionWithOffset;
        ShowCelestialBodyMenu();
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
