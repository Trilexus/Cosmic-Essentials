using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureBuildProgressEntry : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public Image sprite;
    public Slider slider;

    public void Initialize(Area slot)
    {
        UpdateInfos(slot);
        slot.OnConstructionProgressChanged += UpdateInfos;
    }

    private void UpdateInfos(Area slot)
    {
        if (slot.ConstructionProgress >= 100)
        {
            slot.OnConstructionProgressChanged -= UpdateInfos;
            Destroy(gameObject);
            return;
        }
        textObject.text = slot.structure.Name;
        sprite.sprite = slot.structure.Sprite;
        slider.value = slot.ConstructionProgress;      
    }
}
