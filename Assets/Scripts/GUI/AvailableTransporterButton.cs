using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableTransporterButton : MonoBehaviour
{
    [SerializeField]
    public SpacefleetScriptableObject spacefleetScriptableObject;
    private GameObject OrderMenu;
    private OrderMenu orderMenuScript;
    // Start is called before the first frame update
    void Start()
    {
        OrderMenu = GUIManager.Instance.OrderMenu;
        orderMenuScript = OrderMenu.GetComponent<OrderMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {

        orderMenuScript.ChangeSpaceshipSettings(spacefleetScriptableObject);
    }
}
