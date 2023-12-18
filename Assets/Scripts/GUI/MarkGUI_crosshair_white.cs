using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkGUI_crosshair_white : MonoBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public float pulseSpeed = 1.0f;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        // Zugriff auf das Material des Gameobjects
        image = GetComponent<Image>();
    }


    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1);
        image.color = Color.Lerp(startColor, endColor, t);
    }
}

