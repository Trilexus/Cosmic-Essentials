using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    GameObject Background;
    [SerializeField]
    float speed = 2.0f; // Geschwindigkeit der Kamera
    [SerializeField]
    float backgroundSpeed = 0.3f; // Geschwindigkeit des Zooms

    public float zoomSpeed = 1.0f; // Geschwindigkeit des Zooms
    public float minOrthoSize = 3.0f; // Minimale Größe des sichtbaren Bereichs
    public float maxOrthoSize = 15.0f; // Maximale Größe des sichtbaren Bereichs

    private Vector3 mouseStart; // Startpunkt der Bewegung
    private Vector3 mouseEnd; // Endpunkt der Bewegung
    private bool isMoving = false; // Ist die Kamera in Bewegung

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        MoveCameraWithKeys();
        MoveCameraWithMouse();
        ZoomCamera();
    }


    public void MoveCameraWithKeys()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);
        transform.position -= move;
        MoveBackground(move);
        //transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
    }

    public void MoveCameraWithMouse()
    {
        if (Input.GetMouseButtonDown(2) && !isMoving)
        {   
            mouseStart = Input.mousePosition;
            isMoving = true;
        }else if (Input.GetMouseButton(2) && isMoving)
        {
            mouseEnd = Input.mousePosition;
            Vector3 move = mouseEnd - mouseStart;
            mouseStart = Input.mousePosition;
            transform.position -= move * speed * Time.deltaTime;
            MoveBackground(move * Time.deltaTime);            
        } else
        {
            isMoving = false;
        }
    }

    public void MoveBackground(Vector3 move)
    {
        Background.transform.position += move * backgroundSpeed;
    }

    public void ZoomCamera()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        float newOrthoSize = Camera.main.orthographicSize - scrollData * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(newOrthoSize, minOrthoSize, maxOrthoSize);
    }
}
