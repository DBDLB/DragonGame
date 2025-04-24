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
        List<InventoryManager.InventoryPropsData> propsList = new List<InventoryManager.InventoryPropsData>();

        
        foreach (var shelfSlot in shelfSlots)
        {
            Item item = shelfSlot.GetCurrentItem();
            if (item == null) continue;
            switch (item.itemType)
            {
                // 龙蛋
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
                        dragonEgg.sellPrice,
                        dragonEgg.listPrice,
                        dragonEgg.bornDragonId,
                        dragonEgg.bornDragonPro
                        );
                    dragonEggsList.Add(new InventoryManager.InventoryDragonEggData() {itemID = item.itemID, quantity = item.quantity,dragonEggs = eggData});
                    break;
                // 龙
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
                    dragonsList.Add(new InventoryManager.InventoryDragonData() {itemID = item.itemID, quantity = item.quantity,dragons = dragonData});
                    break;
                
                // 战利品
                case ItemType.SpoilsOfWar:
                    SpoilsOfWar spoilsOfWar = item as SpoilsOfWar;
                    SpoilsOfWarData spoilsData = new SpoilsOfWarData
                    (
                        item.id,
                        item.itemName,
                        item.icon.name,
                        spoilsOfWar.description,
                        spoilsOfWar.sellPrice,
                        spoilsOfWar.listPrice,
                        spoilsOfWar.level
                    );
                    spoilsList.Add(new InventoryManager.InventorySpoilsOfWarData() 
                    { 
                        itemID = item.itemID, 
                        quantity = item.quantity, 
                        spoilsOfWar = spoilsData 
                    });
                    break;
                
                case ItemType.Props:
                    Props props = item as Props;
                    PropsData propsData = new PropsData(
                        item.id,
                        item.itemName,
                        item.icon.name,
                        props.description,
                        props.level,
                        props.listPrice,
                        props.sellPrice,
                        props.itemEffect,
                        props.itemEffect1,
                        props.itemEffect2
                    );
                    propsList.Add(new InventoryManager.InventoryPropsData()
                    {
                        itemID = item.itemID,
                        quantity = item.quantity,
                        props = propsData
                    });
                    break;
            }
        }
        
        ShelfDataList inventoryDataList = new ShelfDataList
        {
            inventoryDragonEggs = dragonEggsList.ToArray(),
            inventoryDragons = dragonsList.ToArray(),
            inventorySpoilsOfWar = spoilsList.ToArray(),
            inventoryProps = propsList.ToArray()
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
                        eggData.dragonEggs.sellPrice,
                        eggData.dragonEggs.listPrice
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
                        int.TryParse(dragonData.dragons.life, out int life) ? life : 0,
                        int.TryParse(dragonData.dragons.attack, out int attack) ? attack : 0,
                        int.TryParse(dragonData.dragons.defense, out int defense) ? defense : 0,
                        int.TryParse(dragonData.dragons.speed, out int speed) ? speed : 0,
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
        
            // 加载战利品
            if (inventoryData.inventorySpoilsOfWar != null)
            {
                foreach (var spoilsData in inventoryData.inventorySpoilsOfWar)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + spoilsData.spoilsOfWar.icon);
                    SpoilsOfWar spoils = new SpoilsOfWar(
                        spoilsData.spoilsOfWar.itemName,
                        ItemType.SpoilsOfWar,
                        1,
                        icon,
                        spoilsData.spoilsOfWar.sellPrice,
                        spoilsData.spoilsOfWar.listPrice,
                        spoilsData.spoilsOfWar.level,
                        spoilsData.spoilsOfWar.id,
                        spoilsData.itemID,
                        spoilsData.spoilsOfWar.description
                    );
                    spoils.quantity = spoilsData.quantity;
                    foreach (var shelfSlot in shelfSlots)
                    {
                        if (shelfSlot.GetCurrentItem() == null)
                        {
                            shelfSlot.PlaceItem(spoils);
                            break;
                        }
                    }
                }
            }
            
            // 加载道具
            if (inventoryData.inventoryProps != null)
            {
                foreach (var propsData in inventoryData.inventoryProps)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + propsData.props.icon);
                    Props props = new Props(
                        propsData.props.itemName,
                        ItemType.Props,
                        1,
                        icon,
                        propsData.props.sellPrice,
                        propsData.props.listPrice,
                        propsData.props.level,
                        propsData.props.id,
                        propsData.itemID,
                        propsData.props.description,
                        propsData.props.itemEffect,
                        propsData.props.itemEffect1,
                        propsData.props.itemEffect2
                    );
                    props.quantity = propsData.quantity;
                    foreach (var shelfSlot in shelfSlots)
                    {
                        if (shelfSlot.GetCurrentItem() == null)
                        {
                            shelfSlot.PlaceItem(props);
                            break;
                        }
                    }
                }
            }
        
            Debug.Log("货架数据已加载");
        }
    }
    
    [System.Serializable]
    public class ShelfDataList
    {   
        public InventoryManager.InventoryDragonEggData[] inventoryDragonEggs;
        public InventoryManager.InventoryDragonData[] inventoryDragons;
        public InventoryManager.InventorySpoilsOfWarData[] inventorySpoilsOfWar;
        public InventoryManager.InventoryPropsData[] inventoryProps;  // 添加道具数据
    }
        // 添加更多货架管理功能，如收入计算、自动销售等
    }