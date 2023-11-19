using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffEntry : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public Image sprite;


    public void Initialize(ModifierResourceProductionFactor modifier)
    {
        sprite.sprite = modifier.ModifierSprite;
        textObject.text = modifier.ModifierName + "\nType: " + modifier.ModifierType + " - " + modifier.ResourceType.ToString() + "\nLevel " + modifier.ModifierLevel + " x " + modifier.ResourceProductionFactor;

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
