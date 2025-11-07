using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUIItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Inventory.InventoryItemAndQuantity itemAndQuantity;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI textQuontity;
    private Transform lastPlace, newPlace, canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
    }

    public void SetVisualInfo()
    {
        image.sprite = itemAndQuantity.ItemData.sprite;
        textQuontity.text = itemAndQuantity.Quantity > 1? itemAndQuantity.Quantity.ToString() : "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPlace = transform.parent;
        transform.SetParent(canvas);
        UITooltipSystem.HideTooltip(itemAndQuantity.ItemData.description);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (newPlace)
        {
            if (lastPlace.TryGetComponent(out InventoryUISlot previousSlot))
                if (previousSlot.item == this)
                    previousSlot.item = null;

            if (newPlace.TryGetComponent(out InventoryUISlot newPlaceSlot))
            {
                if (!newPlaceSlot.item)
                {
                    newPlaceSlot.item = this;
                    lastPlace = newPlace;
                }
                else if (newPlaceSlot.item.itemAndQuantity.ItemData == itemAndQuantity.ItemData && newPlaceSlot.item.itemAndQuantity.ItemData.isStackable)
                {
                    newPlaceSlot.item.itemAndQuantity.Quantity += itemAndQuantity.Quantity;
                    newPlaceSlot.item.SetVisualInfo();
                    Destroy(gameObject);
                    return;
                }
            }
            transform.SetParent(newPlace);
        }
        else
            transform.SetParent(lastPlace);

        transform.position = lastPlace.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryUISlot touchedSlot))
        {
            if (!touchedSlot.item || (touchedSlot.item.itemAndQuantity.ItemData == itemAndQuantity.ItemData && touchedSlot.item.itemAndQuantity.ItemData.isStackable))
                newPlace = collision.transform;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryUISlot touchedSlot))
        {
            if (!touchedSlot.item || (touchedSlot.item.itemAndQuantity.ItemData == itemAndQuantity.ItemData && touchedSlot.item.itemAndQuantity.ItemData.isStackable))
                newPlace = collision.transform;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == newPlace)
            newPlace = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UITooltipSystem.ShowTooltip(itemAndQuantity.ItemData.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltipSystem.HideTooltip(itemAndQuantity.ItemData.description);
        UITooltipSystem.HideInfoWindow(itemAndQuantity.ItemData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UITooltipSystem.ShowInfoWindow(itemAndQuantity.ItemData);
    }
}
