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
    [SerializeField] TextMeshProUGUI infoHeader, infoDescription;
    string currentText;
    ItemData currentItem;
    private void Awake()
    {
        _instance = this;
        _instance.tooltip.SetActive(false);
        _instance.infoWindow.SetActive(false);
    }
    public static void ShowTooltip(string text)
    {
        _instance.tooltip.transform.position = Input.mousePosition;
        _instance.currentText = text;
        _instance.tooltipText.text = text;

        _instance.Invoke(nameof(ShowTooltipAfterDelay), 1f);
    }

    void ShowTooltipAfterDelay()
    {
        _instance.tooltip.SetActive(true);
    }

    public static void HideTooltip(string text)
    {
        if (_instance.currentText == text)
        {
            _instance.CancelInvoke(nameof(ShowTooltipAfterDelay));
            _instance.tooltip.SetActive(false);
        }
    }

    public static void ShowInfoWindow(ItemData data)
    {
        _instance.CancelInvoke(nameof(HideWInfoWindowAfterDelay));

        _instance.currentItem = data;

        _instance.infoImage.sprite = data.sprite;
        _instance.infoHeader.text = data.title;
        _instance.infoDescription.text = data.description;

        _instance.infoWindow.SetActive(true);
    }
    public static void HideInfoWindow(ItemData data)
    {
        if (_instance.currentItem == data)
            _instance.Invoke(nameof(HideWInfoWindowAfterDelay), 3f);
    }

    private void HideWInfoWindowAfterDelay()
    {
        _instance.infoWindow.SetActive(false);
    }
}
