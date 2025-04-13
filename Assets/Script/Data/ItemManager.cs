using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ItemManager : MonoBehaviour
{
    // public InstantiatePrefabData itemInstantiatePrefabData;  // 引用 ItemDatabase
    // 为每种物品类型创建单独的字典
    private Dictionary<int, DragonEggData> dragonEggDatabase = new Dictionary<int, DragonEggData>();
    private Dictionary<int, DragonData> dragonDatabase = new Dictionary<int, DragonData>();
    private Dictionary<int, SpoilsOfWarData> spoilsOfWarDatabase = new Dictionary<int, SpoilsOfWarData>();
    
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
        // 加载龙蛋数据
        LoadDragonEggData();
        
        // 加载龙数据
        LoadDragonData();
        
        // 加载战利品数据
        LoadSpoilsOfWarData();
    }
    
    [System.Serializable]
    public class DragonEggDataWrapper
    {
        public DragonEggData[] inventoryDragonEggs { get; set; }
    }
    
    // 加载龙蛋数据
    void LoadDragonEggData()
    {
        string filePath = "Data/dragonEggData";  // 龙蛋JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
    if (jsonText)
    {
        DragonEggData[] itemArray = JsonConvert.DeserializeObject<DragonEggData[]>(jsonText.ToString());
        if (itemArray != null)
        {
            foreach (var item in itemArray)
            {
                dragonEggDatabase.Add(item.id, item);
            }
        }
    }
        else
        {
            Debug.LogError("Dragon Egg JSON file not found at: " + filePath);
        }
    }

    // 加载龙数据
    void LoadDragonData()
    {
        string filePath = "Data/dragonData";  // 龙JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        if (jsonText)
        {
            DragonData[] itemArray = JsonConvert.DeserializeObject<DragonData[]>(jsonText.ToString());
            foreach (var item in itemArray)
            {
                dragonDatabase.Add(item.id, item);
            }
        }
        else
        {
            Debug.LogError("Dragon JSON file not found at: " + filePath);
        }
    }

    // 加载战利品数据
    void LoadSpoilsOfWarData()
    {
        string filePath = "Data/spoilsOfWarData";  // 战利品JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        if (jsonText)
        {
            SpoilsOfWarData[] itemArray = JsonConvert.DeserializeObject<SpoilsOfWarData[]>(jsonText.ToString());
            foreach (var item in itemArray)
            {
                spoilsOfWarDatabase.Add(item.id, item);
            }
        }
        else
        {
            Debug.LogError("Spoils of War JSON file not found at: " + filePath);
        }
    }

// 根据道具ID和类型获取道具数据
    public object GetItemByID(int id, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.DragonEgg:
                if (dragonEggDatabase.ContainsKey(id))
                    return dragonEggDatabase[id];
                break;
            case ItemType.Dragon:
                if (dragonDatabase.ContainsKey(id))
                    return dragonDatabase[id];
                break;
            case ItemType.SpoilsOfWar:
                if (spoilsOfWarDatabase.ContainsKey(id))
                    return spoilsOfWarDatabase[id];
                break;
        }
        Debug.LogWarning($"Item with ID {id} and type {itemType} not found!");
        return null;
    }

    // 实例化道具
    public Item InstantiateItem(int id, ItemType type)
    {
        Item item = null;
        switch (type)
        {
            case ItemType.DragonEgg:
                DragonEggData dragonEggData = (DragonEggData)GetItemByID(id, type);
                if (dragonEggData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + dragonEggData.icon);
                    // List<Vector2> hatchedDragons = new List<Vector2> { dragonEggData.bornDragonA, dragonEggData.bornDragonB };
                    DragonEgg dragonEgg = new DragonEgg(dragonEggData.itemName,type,1,icon,dragonEggData.eggBornTime,dragonEggData.bornDragonId,dragonEggData.bornDragonPro,dragonEggData.id,Item.ItemIDGenerator.GetUniqueID(),dragonEggData.description,dragonEggData.eggModelAdress,dragonEggData.eggPrice);
                    Inventory.Instance.AddItem(dragonEgg);
                    item = dragonEgg;
                }
                break;
            case ItemType.Dragon:
                DragonData dragonData = (DragonData)GetItemByID(id, type);
                if (dragonData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + dragonData.icon);
                    int life = GetRandomValueFromRange(dragonData.life);
                    int attack = GetRandomValueFromRange(dragonData.attack);
                    int defense = GetRandomValueFromRange(dragonData.defense);
                    int speed = GetRandomValueFromRange(dragonData.speed);
                    Item dragon = new Dragon(dragonData.itemName,type,1,icon,life,attack,defense,speed,dragonData.id,Item.ItemIDGenerator.GetUniqueID(),dragonData.description,dragonData.dragonModelAdress);
                    Inventory.Instance.AddItem(dragon);
                    item = dragon;
                }
                break;
            case ItemType.SpoilsOfWar:
                SpoilsOfWarData spoilsOfWarData = (SpoilsOfWarData)GetItemByID(id, type);
                if (spoilsOfWarData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + spoilsOfWarData.icon);
                    // 实例化资源
                }
                break;
            default:
                Debug.LogWarning("Unknown item type: " + type);
                break;
        }
        return item;
    }
    
    private int GetRandomValueFromRange(string rangeString)
    {
        string[] values = rangeString.Split(',');
        if (values.Length == 2 && int.TryParse(values[0].Trim(), out int max) && int.TryParse(values[1].Trim(), out int min))
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
        return 0;
    }
}

// 分别定义各类型数据结构
[System.Serializable]
public class DragonEggData
{
    //通用属性
    public int id;
    public string itemName;
    public string icon;
    public string description;
    
    // 龙蛋特有属性
    public string eggModelAdress;
    public float eggBornTime;
    public float eggPrice;
    public string bornDragonId;
    public string bornDragonPro;
    
    public DragonEggData(int id, string itemName, string icon, string description, string eggModelAdress, float eggBornTime, float eggPrice, string bornDragonId, string bornDragonPro)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.eggModelAdress = eggModelAdress;
        this.eggBornTime = eggBornTime;
        this.eggPrice = eggPrice;
        this.bornDragonId = bornDragonId;
        this.bornDragonPro = bornDragonPro;
    }
    
    // [JsonIgnore]
    // public string[] bornDragonId 
    // {
    //     get { return StringToArray(bornDragonIdString); }
    //     set { bornDragonIdString = ArrayToString(value); }
    // }
    //
    // [JsonIgnore]
    // public string[] bornDragonPro
    // {
    //     get { return StringToArray(bornDragonProString); }
    //     set { bornDragonProString = ArrayToString(value); }
    // }
    
}

[System.Serializable]
public class DragonData
{
    //通用属性
    public int id;
    public string itemName;
    public string icon;
    public string description;
    
    // 龙特有属性
    public string dragonModelAdress;
    public string life;
    public string defense;
    public string attack;
    public string speed;
    
    public DragonData(int id, string itemName, string icon, string description, string dragonModelAdress, string life, string defense, string attack, string speed)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.dragonModelAdress = dragonModelAdress;
        this.life = life;
        this.defense = defense;
        this.attack = attack;
        this.speed = speed;
    }
}

[System.Serializable]
public class SpoilsOfWarData
{
    //通用属性
    public int id;
    public string itemName;
    public string icon;
    public string description;
    
    // 战利品特有属性
    public string isStackable;
    // 战利品特有属性
    public int value;
    // 其他战利品特有属性...
}
