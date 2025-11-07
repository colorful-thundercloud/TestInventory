using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Search;
using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    private static GameDataHolder _instance;
    private ItemData[] items;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
            Destroy(gameObject);

        items = Resources.LoadAll<ItemData>("Items/");
    }

    public static ItemData GetItemByIndex(int index)
    {
        if (index < GetDataLength())
            return _instance.items[index];
        else
            return null;
    }
    
    public static int GetDataLength()
    {
        return _instance.items.Length;
    }
}
