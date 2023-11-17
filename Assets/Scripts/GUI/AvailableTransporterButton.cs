using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableTransporterButton : MonoBehaviour
{
    [SerializeField]
    public SpacefleetScriptableObject spacefleetScriptableObject;
    private GameObject OrderMenu;
    private OrderMenu orderMenuScript;
    [SerializeField]
    bool AutoMode = false;
    // Start is called before the first frame update
    void Start()
    {
        OrderMenu = GUIManager.Instance.OrderMenu;
        orderMenuScript = OrderMenu.GetComponent<OrderMenu>();
        if (AutoMode)
        {
            orderMenuScript.ChangeSpaceshipSettings(spacefleetScriptableObject, AutoMode);
            orderMenuScript.SetActiveButtonColor(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        orderMenuScript.ChangeSpaceshipSettings(spacefleetScriptableObject, AutoMode);
        orderMenuScript.SetActiveButtonColor(gameObject);
    }
}
