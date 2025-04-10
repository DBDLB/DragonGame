using System.Collections.Generic;
using System.IO;
using UnityEngine;
    
    public class ShelfManager : MonoBehaviour
    {
        public static ShelfManager Instance;
        public List<ShelfSlot> shelfSlots = new List<ShelfSlot>();
        public string filePath;
        
        private void Awake()
        {
            filePath = Application.persistentDataPath + "/shelfSlotsData.json";
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            // 获取所有货架槽位
            shelfSlots.AddRange(GetComponentsInChildren<ShelfSlot>());
            LoadShelf();
        }

    public void SaveShelf()
    {
        List<InventoryManager.InventoryDragonEggData> dragonEggsList = new List<InventoryManager.InventoryDragonEggData>();
        List<InventoryManager.InventoryDragonData> dragonsList = new List<InventoryManager.InventoryDragonData>();
        List<InventoryManager.InventorySpoilsOfWarData> spoilsList = new List<InventoryManager.InventorySpoilsOfWarData>();

        foreach (var shelfSlot in shelfSlots)
        {
            Item item = shelfSlot.GetCurrentItem();
            if (item == null) continue;
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
                    dragonEggsList.Add(new InventoryManager.InventoryDragonEggData() {itemID = item.itemID, quantity = item.quantity,dragonEggs = eggData});
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
                        dragon.life,
                        dragon.attack,
                        dragon.defense,
                        dragon.speed
                    );
                    dragonsList.Add(new InventoryManager.InventoryDragonData() {itemID = item.itemID, quantity = item.quantity,dragons = dragonData});
                    break;
                
                case ItemType.SpoilsOfWar:
                    // 类似实现战利品数据保存
                    break;
            }
        }
        
        ShelfDataList inventoryDataList = new ShelfDataList
        {
            inventoryDragonEggs = dragonEggsList.ToArray(),
            inventoryDragons = dragonsList.ToArray(),
            inventorySpoilsOfWar = spoilsList.ToArray()
        };
        string json = JsonUtility.ToJson(inventoryDataList, true);  // 将背包对象序列化为 JSON 字符串
        File.WriteAllText(filePath, json);  // 写入文件
    }


    public void LoadShelf()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ShelfDataList inventoryData = JsonUtility.FromJson<ShelfDataList>(json);
        
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
                    foreach (var shelfSlot in shelfSlots)
                    {
                        if (shelfSlot.GetCurrentItem() == null)
                        {
                            shelfSlot.PlaceItem(egg);
                            break;
                        }
                    }
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
                        dragonData.dragons.life,
                        dragonData.dragons.attack,
                        dragonData.dragons.defense,
                        dragonData.dragons.speed,
                        dragonData.dragons.id,
                        dragonData.itemID,
                        dragonData.dragons.description,
                        dragonData.dragons.dragonModelAdress
                    );
                    dragon.quantity = dragonData.quantity;
                    foreach (var shelfSlot in shelfSlots)
                    {
                        if (shelfSlot.GetCurrentItem() == null)
                        {
                            shelfSlot.PlaceItem(dragon);
                            break;
                        }
                    }
                }
            }
        
            // 加载战利品...
        
            Debug.Log("货架数据已加载");
        }
    }
    
    [System.Serializable]
    public class ShelfDataList
    {   
        public InventoryManager.InventoryDragonEggData[] inventoryDragonEggs;
        public InventoryManager.InventoryDragonData[] inventoryDragons;
        public InventoryManager.InventorySpoilsOfWarData[] inventorySpoilsOfWar;
    }
        // 添加更多货架管理功能，如收入计算、自动销售等
    }