using System.Collections;
using System.Collections.Generic;
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

    public void ShowCelestialBodyMenu(GameObject selectedCelestialBody)
    {
        this.selectedCelestialBody = selectedCelestialBody;
        RectTransform rect = CelestialBodyMenu.GetComponent<RectTransform>();
        celestialBodyMenuOffset = new Vector3(rect.rect.width / 2 * rect.localScale.x, rect.rect.height / -2 * rect.localScale.y, 0);
        Vector3 newPosition = selectedCelestialBody.transform.position + celestialBodyMenuOffset;
        //rect.position = newPosition;
        CelestialBodyMenu.transform.position = newPosition;
        Debug.Log("Celestial Body Position: " + selectedCelestialBody.transform.position);
        Debug.Log("Menu Offset: " + celestialBodyMenuOffset);
        Debug.Log("New Menu Position: " + newPosition);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
