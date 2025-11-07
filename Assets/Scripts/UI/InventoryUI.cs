using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] GameObject slotPrefab, itemPrefab;
    public bool isStorage = false; // чисто для генерации предметов
    [SerializeField] List<InventoryUISlot> slots = new();

    void Start()
    {
        if (isStorage)
            GenerateItems();
        else
        {
            for (int i = 0; i < size.y; i++)
                for (int j = 0; j < size.x; j++)
                {
                    var slot = Instantiate(slotPrefab, transform).GetComponent<InventoryUISlot>();
                    slot.index = new Vector2(j, i);
                    slots.Add(slot);
                }
        }
    }
    // чтобы было откуда брать предметы
    public void GenerateItems()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < size.y; i++)
            for (int j = 0; j < size.x; j++)
            {
                var item = Instantiate(itemPrefab, transform).GetComponent<InventoryUIItem>();
                ItemData randomItem = GameDataHolder.GetItemByIndex(Random.Range(0, GameDataHolder.GetDataLength()));
                item.itemAndQuantity = new Inventory.InventoryItemData(randomItem, randomItem.isStackable ? Random.Range(1, 30) : 1, new Vector2(j, i));
                item.SetVisualInfo();
            }
    }


    public void UseUtem()
    {
        if (!UITooltipSystem.GetSelectedItem()) return;

        InventoryUIItem selectedItem = UITooltipSystem.GetSelectedItem().GetComponent<InventoryUIItem>();
        
        if (!selectedItem.transform.parent.TryGetComponent<InventoryUISlot>(out _))
            return;

        selectedItem.itemAndQuantity.quantity--;
        selectedItem.SetVisualInfo();
        UITooltipSystem.UpdateSelectedItemQuantity();

        if (selectedItem.itemAndQuantity.quantity <= 0)
            DropItem();
    }

    public void DropItem()
    {
        if (!UITooltipSystem.GetSelectedItem()) return;

        InventoryUIItem selectedItem = UITooltipSystem.GetSelectedItem().GetComponent<InventoryUIItem>();

        if (!selectedItem.transform.parent.TryGetComponent<InventoryUISlot>(out _))
            return;

        UITooltipSystem.HideTooltip(selectedItem.itemAndQuantity.itemData.description);
        UITooltipSystem.HideInfoWindowImmidiately();

        if (Inventory.CheckItemExistance(selectedItem.itemAndQuantity))
            Inventory.RemoveItem(selectedItem.itemAndQuantity);

        Destroy(selectedItem.gameObject);
    }

    public void LoadInventory()
    {
        List<Inventory.InventoryItemData> items = Inventory.GetInventoryList();
        items.Sort((a, b) =>
        {
            if (a.index.y == b.index.y)
            {
                return a.index.x.CompareTo(b.index.x);
            }
            return a.index.y.CompareTo(b.index.y);
        });

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < size.y; i++)
            for (int j = 0; j < size.x; j++)
            {
                var slot = Instantiate(slotPrefab, transform).GetComponent<InventoryUISlot>();
                slot.index = new Vector2(j, i);
                slots.Add(slot);
                if (items.Exists(x => x.index == new Vector2(j, i)))
                {
                    var item = Instantiate(itemPrefab, transform).GetComponent<InventoryUIItem>();
                    item.itemAndQuantity = items.Find(x => x.index == new Vector2(j, i));
                    item.SetVisualInfo();

                    item.transform.SetParent(slot.transform);
                    item.transform.position = slot.transform.position;

                    slot.item = item;
                }
            }
    }

}
