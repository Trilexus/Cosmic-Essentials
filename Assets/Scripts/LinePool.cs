using System.Collections.Generic;
using UnityEngine;

public class LinePool : MonoBehaviour
{
    public static LinePool Instance;
    public GameObject linePrefab;
    public int amountToPool;

    private List<GameObject> pooledLines;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledLines = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject lineObj = Instantiate(linePrefab);
            
            // Zugriff auf den LineRenderer
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

            // Dicke der Linie ändern
            lineRenderer.startWidth = 0.02f;  // Dicke am Startpunkt der Linie
            lineRenderer.endWidth = 0.02f;    // Dicke am Endpunkt der Linie
            lineObj.SetActive(false);
            pooledLines.Add(lineObj);
        }
    }

    public GameObject GetPooledLine()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledLines[i].activeInHierarchy)
            {
                return pooledLines[i];
            }
        }
        return null;
    }

    public void ReturnLineToPool(GameObject line)
    {
        line.SetActive(false);
    }
}
