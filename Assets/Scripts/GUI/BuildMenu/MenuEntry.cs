using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuEntry : MonoBehaviour
{
    public TextMeshProUGUI BuildingCounter;
    public TextMeshProUGUI BuildingName;
    public TextMeshProUGUI BuildingInfosCosts;
    public TextMeshProUGUI BuildingInfosThroughput;
    public StringBuilder sb = new StringBuilder();
    [SerializeField]
    protected GameObject BuildProgress;
    [SerializeField]
    protected GameObject BuildProgressItem;
    [SerializeField]
    protected Image Image;

    protected int lastunderConstructionCount = 0;
    protected int underConstructionCount = 0;
    protected int OneHundredPercent = 100;

    // Start is called before the first frame update
    void Start()
    {
        Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool IsBuildable(List<StructureScriptableObject> structureRequirements)
    {
        Structure structure;
        foreach (StructureScriptableObject structureRequirement in structureRequirements)
        {
            structure = EntityManager.Instance.GetStructure(structureRequirement);
            bool hatStrukturMitVollemFortschritt = GUIManager.Instance.selectedCelestialBodyScript.Areas
                .Any(area => area.structure.Type == structure.Type && area.constructionProgress >= 100);
            if (!hatStrukturMitVollemFortschritt)
            {
                return false;
            }
        }
        return true;
    }
}
