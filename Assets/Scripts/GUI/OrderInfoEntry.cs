using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderInfoEntry : MonoBehaviour
{
    public Image OrderOriginImage;
    public TextMeshProUGUI TextOrderOriginName;
    public Image OrderTargetImage;
    public TextMeshProUGUI TextOrderTargetName;
    public TextMeshProUGUI TextOrderTypeAmountInfo;
    public TextMeshProUGUI TextOrderPrioritized;
    public TextMeshProUGUI TextOrderOnlyFullShipment;
    public TextMeshProUGUI TextOrderReturnToOrigin;
    public TextMeshProUGUI TextOrderForever;
    public TextMeshProUGUI TextOrderRepetitions;
    ResourceTransferOrder order;
    public Button ButtonCancelOrder;
    private List<ResourceTransferOrder> resourceTransferOrders;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();
    }

    public void Initialize(ResourceTransferOrder order, List<ResourceTransferOrder> resourceTransferOrders)
    {
        this.resourceTransferOrders = resourceTransferOrders;
        this.order = order;
        UpdateGUI();
    }

    public void CancelOrder()
    {
        if (order != null)
        {
            resourceTransferOrders.Remove(order);
            Destroy(this.gameObject);
        }
    }
    private void UpdateGUI()
    {
        if (order.Repetitions < 0)
        {
            Destroy(this.gameObject);
        }else
        {
            OrderOriginImage.sprite = order.Origin.GetComponent<CelestialBody>().ChildSpriteRenderer.sprite;
            TextOrderOriginName.text = order.Origin.name;
            OrderTargetImage.sprite = order.Target.GetComponent<CelestialBody>().ChildSpriteRenderer.sprite;
            TextOrderTargetName.text = order.Target.name;
            TextOrderTypeAmountInfo.text = Symbols.GetSymbol(order.ResourceShipmentDetails[0].ResourceType) + ": " + order.ResourceShipmentDetails[0].StorageQuantity;
            TextOrderPrioritized.gameObject.SetActive(order.IsPrioritized);
            TextOrderOnlyFullShipment.gameObject.SetActive(order.OnlyFullShipment);
            TextOrderReturnToOrigin.gameObject.SetActive(order.ReturnToOrigin);
            TextOrderForever.gameObject.SetActive(order.IsForever);
            if (order.IsForever)
            {
                TextOrderRepetitions.text = "";
            }else
            {
                TextOrderRepetitions.text = "\uf2f9" + order.Repetitions.ToString();
            }
        }
    }
}
