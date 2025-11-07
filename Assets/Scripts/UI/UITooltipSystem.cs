using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITooltipSystem : MonoBehaviour
{
    private static UITooltipSystem _instance;
    [SerializeField] Canvas canvas;
    [Header("Tooltip")]
    [SerializeField] GameObject tooltip;
    [SerializeField] TextMeshProUGUI tooltipText;
    [Header("Information window")]
    [SerializeField] GameObject infoWindow;
    [SerializeField] Image infoImage;
    [SerializeField] TextMeshProUGUI infoQuantity, infoHeader, infoDescription;
    GameObject selectedItem;
    Inventory.InventoryItemData selectedItemData;
    private void Awake()
    {
        _instance = this;
        _instance.tooltip.SetActive(false);
        _instance.infoWindow.SetActive(false);
    }

    public static void SelectItem(GameObject item)
    {
        _instance.selectedItem = item;
        _instance.selectedItemData = item.GetComponent<InventoryUIItem>().itemAndQuantity;
    }

    public static void DeselectItem(GameObject item)
    {
        if (_instance.selectedItem != item || !_instance.selectedItem)
            return;

        _instance.selectedItem = null;
    }
    // чтоб не загораживало объекты при быстром движении курсора
    public static void ShowTooltipDelayed(string text)
    {
        _instance.tooltip.transform.position = Input.mousePosition;
        _instance.tooltipText.text = text;

        _instance.Invoke(nameof(ShowTooltip), 1f);
    }

    private void ShowTooltip()
    {
        _instance.tooltip.SetActive(true);
    }

    public static void HideTooltip(string text)
    {
        if (_instance.tooltipText.text == text)
        {
            _instance.CancelInvoke(nameof(ShowTooltip));
            _instance.tooltip.SetActive(false);
        }
    }

    public static void ShowInfoWindow(Inventory.InventoryItemData data)
    {
        _instance.CancelInvoke(nameof(HideInfoWindow));

        _instance.infoImage.sprite = data.itemData.sprite;
        _instance.infoQuantity.text = data.quantity.ToString();
        _instance.infoHeader.text = data.itemData.title;
        _instance.infoDescription.text = data.itemData.description;

        _instance.infoWindow.SetActive(true);
    }

    public static void UpdateSelectedItemQuantity()
    {
        if (!_instance.selectedItem) return;
        
        _instance.infoQuantity.text = _instance.selectedItemData.quantity.ToString();
    }

    // чтобы не сразу закрывалось окно
    public static void HideInfoWindowDelayed(Inventory.InventoryItemData data)
    {
        if (!_instance.selectedItem) return;

        if (_instance.selectedItemData == data)
            _instance.Invoke(nameof(HideInfoWindow), 3f);
    }

    public static void HideInfoWindowImmidiately()
    {
        _instance.infoWindow.SetActive(false);
    }

    private void HideInfoWindow()
    {
        _instance.infoWindow.SetActive(false);
    }

    public static GameObject GetSelectedItem()
    {
        return _instance.selectedItem;
    }
}
