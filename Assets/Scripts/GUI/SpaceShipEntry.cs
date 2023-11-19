using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipEntry : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public Image sprite;

    public void Initialize(HangarSlot slot)
    {
        UpdateInfoText(slot);
        slot.OnShipConstructionStatusChanged += UpdateInfoText;
    }

    private void UpdateInfoText(HangarSlot slot)
    {
        sprite.sprite = slot.spacefleetScriptableObject.Sprite;
        string constructionProgress = "";
        if (slot.constructionProgress < 100)
        {
            constructionProgress = " (" + slot.constructionProgress + "%)";
        }
        textObject.text = Symbols.SpaceShip + " - " + slot.spacefleetScriptableObject.name + " - " + Symbols.cargoBox + slot.spacefleetScriptableObject.CargoSpace + constructionProgress;
    }
}
