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
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        canvasGroup = CorrespondingPanel.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {// && button.colors.normalColor != ActiveColor
         //&& button.colors.normalColor != NormalColor
        

        if (canvasGroup.alpha == 1f) {
            SetButtonColor(ActiveColor);
        }else
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
