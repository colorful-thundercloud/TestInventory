using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    public List<InventoryItemAndQuantity> items;
    string itemsJSONpath, itemsJSONcontent;

    [System.Serializable]
    public struct InventoryItemAndQuantity
    {
        private ItemData itemData;
        public ItemData ItemData
        {
            get { return itemData; }
            private set { itemData = value; }
        }
        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        // позиция в UI-инвентаре
        private Vector2 index;
        public Vector2 Index
        {
            get { return index; }
            set { index = value; }
        }

        public InventoryItemAndQuantity(ItemData initialData, int initialQuantity, Vector2 initialIndex)
        {
            itemData = initialData;
            quantity = initialQuantity;
            index = initialIndex;
        }

        public static InventoryItemAndQuantity operator +(InventoryItemAndQuantity itemA, InventoryItemAndQuantity itemB)
        {
            if (itemA.itemData == itemB.itemData && itemA.itemData.isStackable && itemB.itemData.isStackable)
                return new InventoryItemAndQuantity(itemA.itemData, itemA.Quantity + itemB.Quantity, itemA.index);
            return itemA;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        itemsJSONpath = Application.dataPath + "/inventory.json";

        using (FileStream f = File.Open(itemsJSONpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) f.Dispose();       
        
        itemsJSONcontent = File.ReadAllText(itemsJSONpath);
        if (itemsJSONcontent != "") items = JsonUtility.FromJson<List<InventoryItemAndQuantity>>(itemsJSONcontent);
        else items = new();
    }

    public static bool CheckItemExistance(InventoryItemAndQuantity targetItem)
    {
        return _instance.items.Exists(x => x.ItemData == targetItem.ItemData);
    }

    public static InventoryItemAndQuantity GetItem(InventoryItemAndQuantity targetItem)
    {
        return _instance.items.Find(x => x.ItemData == targetItem.ItemData);
    }
    
    public static void AddOrStackItem(InventoryItemAndQuantity newItem)
    {
        if (!CheckItemExistance(newItem))
            _instance.items.Add(newItem);
        else
        {
            if (!newItem.ItemData.isStackable)
                _instance.items.Add(newItem);
            else
            {
                InventoryItemAndQuantity existingItem = GetItem(newItem);
                existingItem.Quantity += newItem.Quantity;
            }
        }
    }

    public static bool RemoveItem(InventoryItemAndQuantity targetItem)
    {
        if (!CheckItemExistance(targetItem)) return false;

        InventoryItemAndQuantity inventoryItem = GetItem(targetItem);

        if (inventoryItem.Quantity == targetItem.Quantity)
            return _instance.items.Remove(inventoryItem);
        else
            inventoryItem.Quantity -= targetItem.Quantity;

        return true;
    }

    public void SaveToJSON()
    {
        itemsJSONcontent = JsonUtility.ToJson(items, true);
        File.WriteAllText(itemsJSONpath, itemsJSONcontent);
    }
    
    public void LoadFromJSON()
    {
        using (FileStream f = File.Open(itemsJSONpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) f.Dispose();       
        
        itemsJSONcontent = File.ReadAllText(itemsJSONpath);
        if (itemsJSONcontent != "") items = JsonUtility.FromJson<List<InventoryItemAndQuantity>>(itemsJSONcontent);
    }
}
