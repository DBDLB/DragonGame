using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    private string filePath;

    public static InventoryManager instance;
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(InventoryManager).Name);
                    instance = singleton.AddComponent<InventoryManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        filePath = Application.persistentDataPath + "/inventory.json";  // 保存路径
        LoadInventory();  // 启动时加载物品
    }

    
    // 保存背包数据到 JSON 文件
    public void SaveInventory()
    {
        List<InventoryData> inventorySaveDataList = new List<InventoryData>();
        foreach (var item in Inventory.Instance.items)
        {
            switch (item.itemType)
            {
                case ItemType.DragonEgg:
                    DragonEgg dragonEgg = item as DragonEgg;
                    InventoryData inventoryData = new InventoryData
                    {
                        id = item.id,
                        itemID = item.itemID,
                        itemName = item.itemName,
                        icon = item.icon.name,
                        itemType = item.itemType.ToString(),
                        isStackable = item.isStackable.ToString(),
                        quantity = item.quantity,
                        incubationTime = dragonEgg.incubationTime,
                        hatchedDragonId = dragonEgg.hatchedDragonId
                    };
                    inventorySaveDataList.Add(inventoryData);
                    break;
                case ItemType.Dragon:
                    Dragon dragon = item as Dragon;
                    InventoryData inventoryData1 = new InventoryData
                    {
                        id = item.id,
                        itemID = item.itemID,
                        itemName = item.itemName,
                        icon = item.icon.name,
                        itemType = item.itemType.ToString(),
                        isStackable = item.isStackable.ToString(),
                        quantity = item.quantity,
                        health = dragon.health,
                        attack = dragon.attack,
                        defense = dragon.defense,
                        speed = dragon.speed
                    };
                    inventorySaveDataList.Add(inventoryData1);
                    break;
            }
        }
        
        InventoryDataList inventoryDataList = new InventoryDataList
        {
            inventoryDataList = inventorySaveDataList.ToArray()
        };
        string json = JsonUtility.ToJson(inventoryDataList, true);  // 将背包对象序列化为 JSON 字符串
        File.WriteAllText(filePath, json);  // 写入文件
        Debug.Log("Inventory saved to: " + filePath);
    }

    // 从 JSON 文件加载背包数据
    public void LoadInventory()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);  // 读取 JSON 字符串
            InventoryDataList inventoryDataList = JsonUtility.FromJson<InventoryDataList>(json);  // 反序列化 JSON 字符串为 Inventory 对象
            if(inventoryDataList.inventoryDataList != null)
            {
                foreach (var item in inventoryDataList.inventoryDataList)
                {
                    ItemManager.Instance.InstantiateInventoryItem(item);
                }
            }
            Debug.Log("Inventory loaded.");
        }
        else
        {
            // 读取默认物品数据（只读）
            TextAsset defaultItems = Resources.Load<TextAsset>("InitialInventory"); // 读取 Resources 里的静态数据

            if (defaultItems)
            {
                InventoryDataList inventoryDataList = JsonUtility.FromJson<InventoryDataList>(defaultItems.ToString());  // 将 JSON 解析为 ItemList
                foreach (var item in inventoryDataList.inventoryDataList)
                {
                    item.itemID = Item.ItemIDGenerator.GetUniqueID();  // 获取唯一ID
                    ItemManager.Instance.InstantiateInventoryItem(item);
                }
            }
            // itemList = JsonUtility.FromJson<ItemList>(defaultItems.text);
            // // ItemManager.Instance.LoadItems(
            // Debug.Log("Inventory file not found, creating new.");
        }
    }

    // 玩家获得物品并保存
    public void AddItemToInventory(Item item)
    {
        // itemList.items.Add(item);
        SaveInventory();  // 每次修改物品后保存
    }
    
    [System.Serializable]
    public class InventoryDataList
    {   
        public InventoryData[] inventoryDataList;
    }
    [System.Serializable]
    public class InventoryData
    {
        public int id;
        public int itemID;
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
        //速度
        public int speed;
    }
    
    
}