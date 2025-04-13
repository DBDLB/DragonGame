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
        List<InventoryDragonEggData> dragonEggsList = new List<InventoryDragonEggData>();
        List<InventoryDragonData> dragonsList = new List<InventoryDragonData>();
        List<InventorySpoilsOfWarData> spoilsList = new List<InventorySpoilsOfWarData>();

        foreach (var item in Inventory.Instance.items)
        {
            switch (item.itemType)
            {
                case ItemType.DragonEgg:
                    DragonEgg dragonEgg = item as DragonEgg;
                    DragonEggData eggData = new DragonEggData
                    (
                        item.id,
                        item.itemName,
                        item.icon.name,
                        dragonEgg.description,
                         
                        
                        // description = dragonEgg.description,
                        // isStackable = item.isStackable.ToString(),
                        dragonEgg.eggModelAdress,
                        dragonEgg.eggBornTime,
                        dragonEgg.eggPrice,
                        dragonEgg.bornDragonId,
                        dragonEgg.bornDragonPro
                        );
                    dragonEggsList.Add(new InventoryDragonEggData() {itemID = item.itemID, quantity = item.quantity,dragonEggs = eggData});
                    break;
                
                case ItemType.Dragon:
                    Dragon dragon = item as Dragon;
                    DragonData dragonData = new DragonData
                    (
                        item.id,
                        item.itemName,
                        item.icon.name,
                        dragon.description,
                        // isStackable = item.isStackable.ToString(),
                        
                        dragon.dragonModelAdress,
                        dragon.life.ToString(),
                        dragon.attack.ToString(),
                        dragon.defense.ToString(),
                        dragon.speed.ToString()
                    );
                    dragonsList.Add(new InventoryDragonData() {itemID = item.itemID, quantity = item.quantity,dragons = dragonData});
                    break;
                
                case ItemType.SpoilsOfWar:
                    // 类似实现战利品数据保存
                    break;
            }
        }
        
        InventoryDataList inventoryDataList = new InventoryDataList
        {
            inventoryDragonEggs = dragonEggsList.ToArray(),
            inventoryDragons = dragonsList.ToArray(),
            inventorySpoilsOfWar = spoilsList.ToArray()
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
            string json = File.ReadAllText(filePath);
            InventoryDataList inventoryData = JsonUtility.FromJson<InventoryDataList>(json);
        
            // 清空当前背包
            Inventory.Instance.items.Clear();
        
            // 加载龙蛋
            if (inventoryData.inventoryDragonEggs != null)
            {
                foreach (var eggData in inventoryData.inventoryDragonEggs)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + eggData.dragonEggs.icon);
                    // List<Vector2> hatchedDragons = new List<Vector2> { eggData.dragonEggs.bornDragonA, eggData.dragonEggs.bornDragonB };
                    DragonEgg egg = new DragonEgg(
                        eggData.dragonEggs.itemName, 
                        ItemType.DragonEgg, 
                        1, 
                        icon, 
                        // bool.Parse(eggData.isStackable), 
                        eggData.dragonEggs.eggBornTime, 
                        eggData.dragonEggs.bornDragonId,
                        eggData.dragonEggs.bornDragonPro,
                        eggData.dragonEggs.id, 
                        eggData.itemID,
                        eggData.dragonEggs.description,
                        eggData.dragonEggs.eggModelAdress,
                        eggData.dragonEggs.eggPrice
                    );
                    egg.quantity = eggData.quantity;
                    Inventory.Instance.AddItem(egg);
                }
            }
        
            // 加载龙
            if (inventoryData.inventoryDragons != null)
            {
                foreach (var dragonData in inventoryData.inventoryDragons)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + dragonData.dragons.icon);
                    Dragon dragon = new Dragon(
                        dragonData.dragons.itemName,
                        ItemType.Dragon,
                        1,
                        icon,
                        // bool.Parse(dragonData.isStackable),
                        int.TryParse(dragonData.dragons.life.ToString(), out int life) ? life : 0,
                        int.TryParse(dragonData.dragons.attack.ToString(), out int attack) ? attack : 0,
                        int.TryParse(dragonData.dragons.defense.ToString(), out int defense) ? defense : 0,
                        int.TryParse(dragonData.dragons.speed.ToString(), out int speed) ? speed : 0,
                        dragonData.dragons.id,
                        dragonData.itemID,
                        dragonData.dragons.description,
                        dragonData.dragons.dragonModelAdress
                    );
                    dragon.quantity = dragonData.quantity;
                    Inventory.Instance.AddItem(dragon);
                }
            }
        
            // 加载战利品...
        
            Debug.Log("背包数据已加载");
        }
        else
        {
            // 读取默认物品数据（只读）
            TextAsset defaultItems = Resources.Load<TextAsset>("InitialInventory"); // 读取 Resources 里的静态数据

            if (defaultItems)
            {
                InventoryDataList inventoryDataList = JsonUtility.FromJson<InventoryDataList>(defaultItems.ToString());  // 将 JSON 解析为 ItemList
                // 加载龙蛋
                if (inventoryDataList.inventoryDragonEggs != null)
                {
                    foreach (var eggData in inventoryDataList.inventoryDragonEggs)
                    {
                        Sprite icon = Resources.Load<Sprite>("Icons/" + eggData.dragonEggs.icon);
                        // List<Vector2> hatchedDragons = new List<Vector2> { eggData.dragonEggs.bornDragonA, eggData.dragonEggs.bornDragonB };
                        DragonEgg egg = new DragonEgg(
                            eggData.dragonEggs.itemName, 
                            ItemType.DragonEgg, 
                            1, 
                            icon, 
                            // bool.Parse(eggData.isStackable), 
                            eggData.dragonEggs.eggBornTime, 
                            eggData.dragonEggs.bornDragonId,
                            eggData.dragonEggs.bornDragonPro,
                            eggData.dragonEggs.id, 
                            Item.ItemIDGenerator.GetUniqueID(),
                            eggData.dragonEggs.description,
                            eggData.dragonEggs.eggModelAdress,
                            eggData.dragonEggs.eggPrice
                        );
                        egg.quantity = eggData.quantity;
                        if (egg.id!=0)
                        {
                            Inventory.Instance.AddItem(egg);
                        }

                    }
                }
                // 加载龙
                if (inventoryDataList.inventoryDragons != null)
                {
                    foreach (var dragonData in inventoryDataList.inventoryDragons)
                    {
                        Sprite icon = Resources.Load<Sprite>("Icons/" + dragonData.dragons.icon);
                        Dragon dragon = new Dragon(
                            dragonData.dragons.itemName,
                            ItemType.Dragon,
                            1,
                            icon,
                            // bool.Parse(dragonData.isStackable),
                            int.TryParse(dragonData.dragons.life.ToString(), out int life) ? life : 0,
                            int.TryParse(dragonData.dragons.attack.ToString(), out int attack) ? attack : 0,
                            int.TryParse(dragonData.dragons.defense.ToString(), out int defense) ? defense : 0,
                            int.TryParse(dragonData.dragons.speed.ToString(), out int speed) ? speed : 0,
                            dragonData.dragons.id,
                            Item.ItemIDGenerator.GetUniqueID(),
                            dragonData.dragons.description,
                            dragonData.dragons.dragonModelAdress
                        );
                        dragon.quantity = dragonData.quantity;
                        Inventory.Instance.AddItem(dragon);
                    }
                }
                // 加载战利品...
                Debug.Log("背包数据已加载");
            }
            // itemList = JsonUtility.FromJson<ItemList>(defaultItems.text);
            // // ItemManager.Instance.LoadItems(
            // Debug.Log("Inventory file not found, creating new.");
        }
    }

    // 玩家获得物品并保存
    public void AddItemToInventory(Item item)
    {
        // 检查是否首次获得此种类物品
        bool isFirstTimeGetting = PlayerDataManager.Instance.IsNewDiscovery(item.id, item.itemType);
    
        if (isFirstTimeGetting)
        {
            // 记录新获得的物品种类
            PlayerDataManager.Instance.MarkItemDiscovered(item.id, item.itemType);
        
            // 这里可以添加首次获得物品的特殊效果
            Debug.Log("首次获得：" + item.itemName + "！");
            // 比如显示提示界面、播放特效等
            NewItemPopup.Instance.items.Add(item);
            NewItemPopup.Instance.ShowPopup();
        }
        // itemList.items.Add(item);
        SaveInventory();  // 每次修改物品后保存
    }
    
    [System.Serializable]
    public class InventoryDataList
    {   
        public InventoryDragonEggData[] inventoryDragonEggs;
        public InventoryDragonData[] inventoryDragons;
        public InventorySpoilsOfWarData[] inventorySpoilsOfWar;
    }

    [System.Serializable]
    public class InventoryDragonEggData
    {
        public int itemID;
        public int quantity;
        public DragonEggData dragonEggs;
    }
    
    [System.Serializable]
    public class InventoryDragonData
    {
        public int itemID;
        public int quantity;
        public DragonData dragons;
    }
    
    [System.Serializable]
    public class InventorySpoilsOfWarData
    {
        public int itemID;
        public int quantity;
        public SpoilsOfWarData spoilsOfWar;
    }
    
    
    // [System.Serializable]
    // public class InventoryData
    // {
    //     public int id;
    //     public int itemID;
    //     public string itemName;
    //     public string description;
    //     public string icon;
    //     public string itemType;
    //     public string isStackable; 
    //     public int quantity;
    //
    //     //龙蛋
    //     public float eggBornTime;
    //     public int hatchedDragonId;
    //
    //     //龙
    //     //生命
    //     public int life;
    //     //攻击力
    //     public int attack;
    //     //防御力
    //     public int defense;
    //     //速度
    //     public int speed;
    // }
    
    
}