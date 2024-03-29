using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsMenu : MonoBehaviour
{
    [SerializeField]
    List<Button> buttons = new List<Button>();
    private List<GameObject> panels = new List<GameObject>();
    public delegate void OnTabActivate(GameObject panel);
    public event OnTabActivate onTabActivate;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Button button in buttons)
        {
            panels.Add(button.GetComponent<ButtonData>().CorrespondingPanel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(Button button)
    {
        //foreach (GameObject panel in panels)
        //{
        //    panel.SetActive(false);
        //}
        //button.GetComponent<ButtonData>().CorrespondingPanel.SetActive(true);
        ActivatePanel(button.GetComponent<ButtonData>().CorrespondingPanel);
    }

    public void ActivatePanel(GameObject aktivatePanel)
    {
        onTabActivate?.Invoke(aktivatePanel);
        CanvasGroup canvasGroup;
        foreach (GameObject panel in panels)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        canvasGroup = aktivatePanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
