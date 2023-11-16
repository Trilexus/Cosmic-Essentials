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
        sprite.sprite = slot.spacefleetScriptableObject.Sprite;
        textObject.text = Symbols.SpaceShip + " - " + slot.spacefleetScriptableObject.name + " - " +Symbols.cargoBox +  slot.spacefleetScriptableObject.MaxCargoSpace;
    }
}
