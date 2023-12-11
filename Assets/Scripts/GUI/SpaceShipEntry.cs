using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceShipEntry : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI textObject;
    public TextMeshProUGUI textFuel;
    public Image sprite;
    public delegate void ClickAction(SpacefleetScriptableObject spacefleetScriptableObject);
    public static event ClickAction OnClickEvent;
    private HangarSlot slot;

    public void Initialize(HangarSlot slot)
    {
        this.slot = slot;
        UpdateInfoText(slot);
        slot.OnShipStatusChanged += UpdateInfoText;
    }

    public void Initialize(SpaceShip spaceShip)
    {
        UpdateInfoText(spaceShip);
        //spaceShip.OnShipStatusChanged += UpdateInfoText;
    }

    private void UpdateInfoText(HangarSlot slot)
    {
        sprite.sprite = slot.spacefleetScriptableObject.Sprite;
        string constructionProgress = "";
        if (slot.constructionProgress < 100)
        {
            constructionProgress = " (" + slot.constructionProgress + "%)";
        }
        textObject.text = Symbols.SpaceShip + " - " + slot.spacefleetScriptableObject.Name + " - " + Symbols.cargoBox + slot.spacefleetScriptableObject.CargoSpace + constructionProgress;
        textFuel.text = Symbols.fuel + slot.spacefleetScriptableObject.Fuel + "/" + slot.spacefleetScriptableObject.MaxFuel;
    }

    private void UpdateInfoText(SpaceShip spaceship)
    {
        sprite.sprite = spaceship.SpacefleetScriptableObject.Sprite;
        textObject.text = Symbols.SpaceShip + " - " + spaceship.SpacefleetScriptableObject.Name + " - " + Symbols.cargoBox + spaceship.ResourceStorage;
        textFuel.text = Symbols.fuel + spaceship.SpacefleetScriptableObject.Fuel + "/" + spaceship.SpacefleetScriptableObject.MaxFuel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(slot.spacefleetScriptableObject);
        Debug.Log("SpaceShipEntry.OnPointerClick");
    }

    public void OnDestroy()
    {
        slot.OnShipStatusChanged -= UpdateInfoText;
    }
}
