using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    [SerializeField]
    public List<InventoryItemData> items = new();
    string itemsJSONpath, itemsJSONcontent;

    [System.Serializable]
    public class InventoryItemData
    {
        public ItemData itemData;
        public int quantity;
        // позиция в UI-инвентаре
        public Vector2 index;

        public InventoryItemData(ItemData initialData, int initialQuantity, Vector2 initialIndex)
        {
            itemData = initialData;
            quantity = initialQuantity;
            index = initialIndex;
        }

        public static InventoryItemData operator +(InventoryItemData itemA, InventoryItemData itemB)
        {
            if (itemA.itemData == itemB.itemData && itemA.itemData.isStackable)
                return new InventoryItemData(itemA.itemData, itemA.quantity + itemB.quantity, itemA.index);
            return itemA;
        }

        public static bool operator ==(InventoryItemData itemA, InventoryItemData itemB)
        {
            if (itemA.itemData == itemB.itemData && itemA.quantity == itemB.quantity)
                return true;
            return false;
        }

        public static bool operator !=(InventoryItemData itemA, InventoryItemData itemB)
        {
            if (itemA.itemData == itemB.itemData && itemA.quantity == itemB.quantity)
                return false;
            return true;
        }
    }
    // костыль для json utility, который не может в сериализацию списков
    [System.Serializable]
    public class ItemsListWrapper
    {
        public List<InventoryItemData> list;
    }

    ItemsListWrapper wrapper = new();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        itemsJSONpath = Application.dataPath + "/inventory.json";

        using (FileStream f = File.Open(itemsJSONpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) f.Dispose();       
        
        itemsJSONcontent = File.ReadAllText(itemsJSONpath);
        if (itemsJSONcontent != "") items = JsonUtility.FromJson<List<InventoryItemData>>(itemsJSONcontent);
        else items = new();
    }

    public static bool CheckItemExistance(InventoryItemData targetItem)
    {
        return _instance.items.Exists(x => x == targetItem && x.index == targetItem.index);
    }

    public static InventoryItemData GetItem(InventoryItemData targetItem)
    {
        return _instance.items.Find(x => x == targetItem && x.index == targetItem.index);
    }

    public static void AddItem(InventoryItemData newItem)
    {
        _instance.items.Add(newItem);
    }

    public static void StackItem(InventoryItemData targetItem, int addingValue)
    {
        if (CheckItemExistance(targetItem) && targetItem.itemData.isStackable)
        {
            InventoryItemData existingItem = GetItem(targetItem);
            existingItem.quantity += addingValue;
        }
    }

    public static bool RemoveItem(InventoryItemData targetItem)
    {
        if (!CheckItemExistance(targetItem)) return false;

        InventoryItemData inventoryItem = GetItem(targetItem);
        return _instance.items.Remove(inventoryItem);
    }

    public static List<InventoryItemData> GetInventoryList()
    {
        return _instance.items;
    }

    public void SaveToJSON()
    {
        wrapper.list = items;
        itemsJSONcontent = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(itemsJSONpath, itemsJSONcontent);
    }
    
    public void LoadFromJSON()
    {
        using (FileStream f = File.Open(itemsJSONpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) f.Dispose();       
        
        itemsJSONcontent = File.ReadAllText(itemsJSONpath);
        if (itemsJSONcontent != "") wrapper = JsonUtility.FromJson<ItemsListWrapper>(itemsJSONcontent);
        items = wrapper.list;
    }
}
