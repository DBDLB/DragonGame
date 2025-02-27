using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    private Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>(); // 道具数据库

    void Start()
    {
        LoadItems();  // 启动时加载道具数据
    }

    // 从 JSON 文件加载道具配置
    void LoadItems()
    {
        string filePath = "items";  // JSON 文件路径
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);  // JSON 文件路径
        if (jsonText)
        {
            ItemList itemList = JsonUtility.FromJson<ItemList>(jsonText.ToString());  // 将 JSON 解析为 ItemList

            foreach (var item in itemList.items)
            {
                itemDatabase.Add(item.id, item);  // 将道具信息加入字典
            }
        }
        else
        {
            Debug.LogError("Items JSON file not found at: " + filePath);
        }
    }

    // 根据道具ID获取道具数据
    public ItemData GetItemByID(int id)
    {
        if (itemDatabase.ContainsKey(id))
        {
            return itemDatabase[id];
        }
        else
        {
            Debug.LogWarning("Item with ID " + id + " not found!");
            return null;
        }
    }

    // 实例化道具
    public void InstantiateItem(int id)
    {
        ItemData itemData = GetItemByID(id);
        if (itemData != null)
        {
            switch (itemData.type)
            {
                case "DragonEgg":
                    // 实例化龙蛋
                    
                    
                    break;
                case "Resource":
                    // 实例化资源
                    break;
                case "Equipment":
                    // 实例化装备
                    break;
                default:
                    Debug.LogWarning("Unknown item type: " + itemData.type);
                    break;
            }
        }
    }
}

// 用于解析 JSON 数据的辅助类
[System.Serializable]
public class ItemList
{
    public ItemData[] items;
}

[System.Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public string type;
    public string isStackable;
    public int value;
}