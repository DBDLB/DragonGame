using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    // public InstantiatePrefabData itemInstantiatePrefabData;  // 引用 ItemDatabase
    private Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>(); // 道具数据库
    
    public static ItemManager instance;
    public static ItemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ItemManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(ItemManager).Name);
                    instance = singleton.AddComponent<ItemManager>();
                }
            }
            return instance;
        }
    }

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
    public Item InstantiateItem(int id)
    {
        ItemData itemData = GetItemByID(id);
        Item item = null;
        if (itemData != null)
        {
            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), itemData.itemType);
            int itemID = Item.ItemIDGenerator.GetUniqueID(); // 获取唯一ID
            switch (itemType)
            {
                case ItemType.DragonEgg:
                    // 实例化龙蛋
                    Sprite icon = Resources.Load<Sprite>("Icons/" + itemData.icon);
                    DragonEgg dragonEgg = new DragonEgg(itemData.itemName,itemType,1,icon,bool.Parse(itemData.isStackable),itemData.incubationTime,itemData.hatchedDragonId,itemData.id,itemID);
                    Inventory.Instance.AddItem(dragonEgg);
                    item = dragonEgg;
                    break;
                case ItemType.Resource:
                    // 实例化资源
                    break;
                case ItemType.Dragon:
                    // 实例化龙
                    Sprite dragonIcon = Resources.Load<Sprite>("Icons/" + itemData.icon);
                    Item dragon = new Dragon(itemData.itemName,itemType,1,dragonIcon,bool.Parse(itemData.isStackable),itemData.health,itemData.attack,itemData.defense,itemData.id,itemID);
                    Inventory.Instance.AddItem(dragon);
                    item = dragon;
                    break;
                default:
                    Debug.LogWarning("Unknown item type: " + itemData.itemType);
                    break;
            }
        }
        return item;
    }
    public Item InstantiateInventoryItem(InventoryManager.InventoryData inventoryData)
    {
        Item item = null;
        ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), inventoryData.itemType);
        switch (itemType)
        {
            case ItemType.DragonEgg:
                // 实例化龙蛋
                Sprite icon = Resources.Load<Sprite>("Icons/" + inventoryData.icon);
                DragonEgg dragonEgg = new DragonEgg(inventoryData.itemName,itemType,inventoryData.quantity,icon,bool.Parse(inventoryData.isStackable),inventoryData.incubationTime,inventoryData.hatchedDragonId,inventoryData.id,inventoryData.itemID);
                Inventory.Instance.AddItem(dragonEgg);
                item = dragonEgg;
                break;
            case ItemType.Resource:
                // 实例化资源
                break;
            case ItemType.Dragon:
                // 实例化龙
                Sprite dragonIcon = Resources.Load<Sprite>("Icons/" + inventoryData.icon);
                Item dragon = new Dragon(inventoryData.itemName,itemType,inventoryData.quantity,dragonIcon,bool.Parse(inventoryData.isStackable),inventoryData.health,inventoryData.attack,inventoryData.defense,inventoryData.id,inventoryData.itemID);
                Inventory.Instance.AddItem(dragon);
                item = dragon;
                break;
            default:
                Debug.LogWarning("Unknown item type: " + inventoryData.itemType);
                break;
        }
        return item;
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
    public string itemName;
    public string description;
    public string icon;
    public string itemType;
    public string isStackable; 
    public int quantity;
    
    //龙蛋
    public float incubationTime;
    public int hatchedDragonId;
    
    //龙
    //生命
    public int health;
    //攻击力
    public int attack;
    //防御力
    public int defense;
   
}