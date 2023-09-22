using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float zoomSpeed = 1.0f; // Geschwindigkeit des Zooms
    public float minOrthoSize = 3.0f; // Minimale Größe des sichtbaren Bereichs
    public float maxOrthoSize = 15.0f; // Maximale Größe des sichtbaren Bereichs

    void Update()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        float newOrthoSize = Camera.main.orthographicSize - scrollData * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(newOrthoSize, minOrthoSize, maxOrthoSize);
    }
}
