using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsMenu : MonoBehaviour
{
    [SerializeField]
    List<Button> buttons = new List<Button>();
    private List<GameObject> panels = new List<GameObject>();

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
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        button.GetComponent<ButtonData>().CorrespondingPanel.SetActive(true);
    }
}
