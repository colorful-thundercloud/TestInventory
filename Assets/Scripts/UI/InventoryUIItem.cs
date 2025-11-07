using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUIItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Inventory.InventoryItemData itemAndQuantity;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI textQuontity;
    private Transform lastPlace, newPlace, canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
    }

    public void SetVisualInfo()
    {
        image.sprite = itemAndQuantity.itemData.sprite;
        textQuontity.text = itemAndQuantity.itemData.isStackable ? itemAndQuantity.quantity.ToString() : "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPlace = transform.parent;
        transform.SetParent(canvas); // чтобы перетаскиваемый предмет был поверх интерфейса
        UITooltipSystem.HideTooltip(itemAndQuantity.itemData.description);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        UITooltipSystem.HideTooltip(itemAndQuantity.itemData.description);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!newPlace)
            transform.SetParent(lastPlace);
        else
        {
            if (!lastPlace.TryGetComponent(out InventoryUISlot previousSlot))
            {
                if (newPlace.TryGetComponent(out InventoryUISlot newPlaceSlot))
                {
                    if (!newPlaceSlot.item)
                    {
                        newPlaceSlot.item = this;
                        itemAndQuantity.index = newPlaceSlot.index;
                        lastPlace = newPlace;
                        Inventory.AddItem(itemAndQuantity);
                    }
                    else
                    {
                        if (newPlaceSlot.item.itemAndQuantity.itemData == itemAndQuantity.itemData && itemAndQuantity.itemData.isStackable)
                        {
                            Inventory.StackItem(newPlaceSlot.item.itemAndQuantity, itemAndQuantity.quantity);

                            newPlaceSlot.item.SetVisualInfo();
                            UITooltipSystem.SelectItem(newPlaceSlot.item.gameObject);
                            UITooltipSystem.ShowInfoWindow(newPlaceSlot.item.itemAndQuantity);

                            Destroy(gameObject);
                        }
                    }
                }
            }
            else
            {
                if (newPlace.TryGetComponent(out InventoryUISlot newPlaceSlot))
                {
                    if (!newPlaceSlot.item)
                    {
                        if (previousSlot.item == this)
                            previousSlot.item = null;

                        newPlaceSlot.item = this;
                        itemAndQuantity.index = newPlaceSlot.index;

                        lastPlace = newPlace;
                    }
                    else
                    {
                        if (newPlaceSlot.item.itemAndQuantity.itemData == itemAndQuantity.itemData && itemAndQuantity.itemData.isStackable)
                        {
                            if (previousSlot.item == this)
                                previousSlot.item = null;

                            Inventory.StackItem(newPlaceSlot.item.itemAndQuantity, itemAndQuantity.quantity);

                            newPlaceSlot.item.SetVisualInfo();
                            UITooltipSystem.SelectItem(newPlaceSlot.item.gameObject);
                            UITooltipSystem.ShowInfoWindow(newPlaceSlot.item.itemAndQuantity);

                            Inventory.RemoveItem(itemAndQuantity);
                            Destroy(gameObject);
                        }
                    }
                }
            }
            
            transform.SetParent(newPlace);
        }

        transform.position = lastPlace.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryUISlot touchedSlot))
        {
            if (!touchedSlot.item || (touchedSlot.item.itemAndQuantity.itemData == itemAndQuantity.itemData && touchedSlot.item.itemAndQuantity.itemData.isStackable))
                newPlace = collision.transform;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryUISlot touchedSlot))
        {
            if (!touchedSlot.item || (touchedSlot.item.itemAndQuantity.itemData == itemAndQuantity.itemData && touchedSlot.item.itemAndQuantity.itemData.isStackable))
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
        UITooltipSystem.ShowTooltipDelayed(itemAndQuantity.itemData.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltipSystem.HideTooltip(itemAndQuantity.itemData.description);
        UITooltipSystem.HideInfoWindowDelayed(itemAndQuantity);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UITooltipSystem.SelectItem(gameObject);
        UITooltipSystem.ShowInfoWindow(itemAndQuantity);
    }
}
