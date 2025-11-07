using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    [SerializeField] GameObject slotPrefab, itemPrefab;
    public bool isStorage = false;
    public List<InventoryUISlot> slots = new();
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
                item.itemAndQuantity = new Inventory.InventoryItemAndQuantity(randomItem, randomItem.isStackable ? Random.Range(1, 30) : 1, new Vector2(j, i));
                item.SetVisualInfo();
            }
    }

}
