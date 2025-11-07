using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "New item", order = 0)]
public class ItemData : ScriptableObject
{
    public string title;
    [TextArea]
    public string description;
    [SerializeField] public Sprite sprite;
    public ItemType type;
    public bool isStackable;

    [System.Serializable]
    public enum ItemType
    {
        Weapon, Tool, Consumable, Resource
    }
}
