using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
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
                    List<Vector2> hatchedDragons = new List<Vector2> { dragonEggData.bornDragonA, dragonEggData.bornDragonB };
                    DragonEgg dragonEgg = new DragonEgg(dragonEggData.itemName,type,1,icon,dragonEggData.eggBornTime,hatchedDragons,dragonEggData.id,Item.ItemIDGenerator.GetUniqueID(),dragonEggData.description,dragonEggData.eggModelAdress,dragonEggData.eggPrice);
                    Inventory.Instance.AddItem(dragonEgg);
                    item = dragonEgg;
                }
                break;
            case ItemType.Dragon:
                DragonData dragonData = (DragonData)GetItemByID(id, type);
                if (dragonData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + dragonData.icon);
                    Item dragon = new Dragon(dragonData.itemName,type,1,icon,dragonData.life,dragonData.attack,dragonData.defense,dragonData.speed,dragonData.id,Item.ItemIDGenerator.GetUniqueID(),dragonData.description,dragonData.dragonModelAdress);
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
    public string bornDragonAString;
    public string bornDragonBString;
    
    public DragonEggData(int id, string itemName, string icon, string description, string eggModelAdress, float eggBornTime, float eggPrice, Vector2 bornDragonA, Vector2 bornDragonB)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.eggModelAdress = eggModelAdress;
        this.eggBornTime = eggBornTime;
        this.eggPrice = eggPrice;
        this.bornDragonA = bornDragonA;
        this.bornDragonB = bornDragonB;
    }
    
    // 不序列化的Vector2属性
    [JsonIgnore]
    public Vector2 bornDragonA 
    {
        get { return StringToVector2(bornDragonAString); }
        set { bornDragonAString = Vector2ToString(value); }
    }
    
    [JsonIgnore]
    public Vector2 bornDragonB
    {
        get { return StringToVector2(bornDragonBString); }
        set { bornDragonBString = Vector2ToString(value); }
    }
    
    // 辅助方法：将字符串转换为Vector2
    private Vector2 StringToVector2(string vectorString)
    {
        if (string.IsNullOrEmpty(vectorString))
            return Vector2.zero;
            
        string[] values = vectorString.Split(',');
        if (values.Length != 2)
            return Vector2.zero;
            
        float x, y;
        if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y))
            return new Vector2(x, y);
            
        return Vector2.zero;
    }
    
    // 辅助方法：将Vector2转换为字符串
    private string Vector2ToString(Vector2 vector)
    {
        return vector.x + "," + vector.y;
    }
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
    public int life;
    public int defense;
    public int attack;
    public int speed;
    
    public DragonData(int id, string itemName, string icon, string description, string dragonModelAdress, int life, int defense, int attack, int speed)
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
