using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CelestialBodyInfo : MonoBehaviour
{
    GameObject selectedCelestialBody;
    GameObject celestialBodyInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCelestialBodyInfos()
    {
        celestialBodyInfo = GUIManager.Instance.CelestialBodyBuildInfo;
        selectedCelestialBody = GUIManager.Instance.selectedCelestialBody;
        if (selectedCelestialBody != null && celestialBodyInfo.activeSelf)
        {
            foreach (Transform child in celestialBodyInfo.transform)
            {
                Destroy(child.gameObject);
            }
            Vector3 CelestialBodyInfoSymbolOffset = new Vector3(60, 0, 0);
            RectTransform InfoRect = celestialBodyInfo.GetComponent<RectTransform>();
            Vector3 SymbolPosition = new Vector3(InfoRect.position.x, InfoRect.position.y, 0);
            foreach (Area area in selectedCelestialBody.GetComponent<CelestialBody>().Areas.Where<Area>(i => i.ConstructionProgress < 100))
            {
                GameObject Symbol = Instantiate(area.structure.Symbol);
                Symbol.transform.SetParent(celestialBodyInfo.transform);
                Symbol.transform.position = SymbolPosition;
                Symbol.GetComponent<RectMask2D>().padding = new Vector4(0, 0, 0, 45 - (0.45f * Mathf.Max(area.ConstructionProgress,10)));
                SymbolPosition = new Vector3(SymbolPosition.x + CelestialBodyInfoSymbolOffset.x, SymbolPosition.y, 0);
            }
        }
    }
}