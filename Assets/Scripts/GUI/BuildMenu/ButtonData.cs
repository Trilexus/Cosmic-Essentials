using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonData : MonoBehaviour
{
    
    public GameObject CorrespondingPanel;
    private Button button;
    public Color ActiveColor;
    public Color NormalColor;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {// && button.colors.normalColor != ActiveColor
         //&& button.colors.normalColor != NormalColor
           
        if (CorrespondingPanel.activeSelf) {
            SetButtonColor(ActiveColor);
        }else if (!CorrespondingPanel.activeSelf)
        {
            SetButtonColor(NormalColor);
        }
    }

    public void SetButtonColor(Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        button.colors = cb;
    }
}
