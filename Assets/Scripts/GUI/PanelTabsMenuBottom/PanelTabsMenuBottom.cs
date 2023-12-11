using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class PanelTabsMenuBottom : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Tabs;
    List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject tab in Tabs)
        {
            canvasGroups.Add(tab.GetComponent<CanvasGroup>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ActivateTab(GameObject gameObject)
    {
        foreach (CanvasGroup canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.GetComponent<CanvasGroup>().alpha = 1;
        gameObject.GetComponent<CanvasGroup>().interactable = true;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
