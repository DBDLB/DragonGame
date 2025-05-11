using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    // public InstantiatePrefabData itemInstantiatePrefabData;  // 引用 ItemDatabase
    // 为每种物品类型创建单独的字典
    private Dictionary<int, DragonEggData> dragonEggDatabase = new Dictionary<int, DragonEggData>();
    private Dictionary<int, DragonData> dragonDatabase = new Dictionary<int, DragonData>();
    private Dictionary<int, SpoilsOfWarData> spoilsOfWarDatabase = new Dictionary<int, SpoilsOfWarData>();
    private Dictionary<int, PropsData> propsDatabase = new Dictionary<int, PropsData>();
    private Dictionary<int, TermPoolData> termPoolDatabase = new Dictionary<int, TermPoolData>();
    private Dictionary<int, DragonEntryListData> dragonEntryListDatabase = new Dictionary<int, DragonEntryListData>();
    
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
        
        // 加载道具数据
        LoadPropsData();
        
        // 加载词条池数据
        LoadTermPoolData();
        
        // 加载龙词条表数据
        LoadDragonEntryListData();
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
    
    void LoadPropsData()
    {
        string filePath = "Data/gamePropsData";  // 道具JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        if (jsonText)
        {
            PropsData[] itemArray = JsonConvert.DeserializeObject<PropsData[]>(jsonText.ToString());
            foreach (var item in itemArray)
            {
                propsDatabase.Add(item.id, item);
            }
        }
        else
        {
            Debug.LogError("Props JSON file not found at: " + filePath);
        }
    }
    
    //加载词条池表
    void LoadTermPoolData()
    {
        string filePath = "Data/termPoolData";  // 词条池JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        if (jsonText)
        {
            TermPoolData[] itemArray = JsonConvert.DeserializeObject<TermPoolData[]>(jsonText.ToString());
            foreach (var item in itemArray)
            {
                termPoolDatabase.Add(item.id, item);
            }
        }
        else
        {
            Debug.LogError("Term Pool JSON file not found at: " + filePath);
        }
    }
    
    //加载
    void LoadDragonEntryListData()
    {
        string filePath = "Data/dragonEntryListData";  // 词条池JSON文件
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        if (jsonText)
        {
            DragonEntryListData[] itemArray = JsonConvert.DeserializeObject<DragonEntryListData[]>(jsonText.ToString());
            foreach (var item in itemArray)
            {
                dragonEntryListDatabase.Add(item.id, item);
            }
        }
        else
        {
            Debug.LogError("Dragon Entry List JSON file not found at: " + filePath);
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
            case ItemType.Props:
                if (propsDatabase.ContainsKey(id))
                    return propsDatabase[id];
                break;
        }
        Debug.LogWarning($"Item with ID {id} and type {itemType} not found!");
        return null;
    }
    
    public Item InstantiateItem(int id)
    {
        // 获取前三位数
        int typeCode = id / 10;  // 先除以10去掉最后一位
        while (typeCode >= 1000)  // 如果超过3位，继续除以10
        {
            typeCode /= 10;
        }

        // 根据前三位确定物品类型
        ItemType type;
        switch (typeCode)
        {
            case 101:  // 龙蛋
                type = ItemType.DragonEgg;
                break;
            case 102:  // 龙
                type = ItemType.Dragon;
                break;
            case 103:  // 战利品
                type = ItemType.SpoilsOfWar;
                break;
            case 104:  // 道具
                type = ItemType.Props;
                break;
            default:
                Debug.LogWarning($"Unknown item type code: {typeCode} for ID: {id}");
                return null;
        }

        return InstantiateItem(id, type);
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
                    DragonEgg dragonEgg = new DragonEgg(dragonEggData.itemName,type,1,icon,dragonEggData.eggBornTime,dragonEggData.bornDragonId,dragonEggData.bornDragonPro,dragonEggData.id,Item.ItemIDGenerator.GetUniqueID(),dragonEggData.description,dragonEggData.eggModelAdress,dragonEggData.sellPrice,dragonEggData.listPrice);
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
                    
                    // 为每个属性位置随机生成词条
                    string finalAttributeA = GetRandomEntryFromPool(dragonData.attributeA);
                    string finalAttributeB = GetRandomEntryFromPool(dragonData.attributeB);
                    string finalAttributeC = GetRandomEntryFromPool(dragonData.attributeC);
                    
                    Item dragon = new Dragon(
                        dragonData.itemName,
                        type,
                        1,
                        icon,
                        life,
                        attack,
                        defense,
                        speed,
                        dragonData.id,
                        Item.ItemIDGenerator.GetUniqueID(),
                        dragonData.description,
                        dragonData.dragonModelAdress,
                        dragonData.level,
                        finalAttributeA,  // 使用随机生成的词条
                        finalAttributeB,
                        finalAttributeC
                    );
                    Inventory.Instance.AddItem(dragon);
                    item = dragon;
                }
                break;
            case ItemType.SpoilsOfWar:
                SpoilsOfWarData spoilsOfWarData = (SpoilsOfWarData)GetItemByID(id, type);
                if (spoilsOfWarData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + spoilsOfWarData.icon);
                    Item spoilsOfWar = new SpoilsOfWar(
                        spoilsOfWarData.itemName,
                        type,
                        1,
                        icon,
                        spoilsOfWarData.sellPrice,
                        spoilsOfWarData.listPrice,
                        spoilsOfWarData.level,
                        spoilsOfWarData.id,
                        Item.ItemIDGenerator.GetUniqueID(),
                        spoilsOfWarData.description
                    );
                    Inventory.Instance.AddItem(spoilsOfWar);
                    item = spoilsOfWar;
                }
                break;
            case ItemType.Props:
                PropsData propsData = (PropsData)GetItemByID(id, type);
                if (propsData != null)
                {
                    Sprite icon = Resources.Load<Sprite>("Icons/" + propsData.icon);
                    Item props = new Props(
                        propsData.itemName,
                        type,
                        1,
                        icon,
                        propsData.sellPrice,
                        propsData.listPrice,
                        propsData.level,
                        propsData.id,
                        Item.ItemIDGenerator.GetUniqueID(),
                        propsData.description,
                        propsData.itemEffect,
                        propsData.itemEffect1,
                        propsData.itemEffect2
                    );
                    Inventory.Instance.AddItem(props);
                    item = props;
                }
                break;
        }
        return item;
    }
    
    // 根据词条池ID获取随机词条ID
    private string GetRandomEntryFromPool(string termPoolId)
    {
        // 获取词条池数据
        if (int.TryParse(termPoolId, out int poolId) && termPoolDatabase.TryGetValue(poolId, out TermPoolData poolData))
        {
            // 获取词条ID和概率数组
            string[] entryIds = poolData.GetAttributePortals();
            float[] probabilities = poolData.GetAttributeProbabilities();

            if (entryIds.Length == 0 || entryIds.Length != probabilities.Length)
                return "0";

            // 生成0-100之间的随机值
            float randomValue = Random.Range(0f, 100f);
        
            // 累加概率并判断
            float currentSum = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                currentSum += probabilities[i];
                if (randomValue <= currentSum)
                {
                    return entryIds[i];
                }
            }
        
            // 如果没有命中任何词条，返回0表示空词条
            return "0";
        }

        return "0";
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
    public float sellPrice;
    public float listPrice;
    public string bornDragonId;
    public string bornDragonPro;
    
    public DragonEggData(int id, string itemName, string icon, string description, string eggModelAdress, float eggBornTime, float sellPrice,float listPrice, string bornDragonId, string bornDragonPro)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.eggModelAdress = eggModelAdress;
        this.eggBornTime = eggBornTime;
        this.sellPrice = sellPrice;
        this.listPrice = listPrice;
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
    // 通用属性
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
    
    // 添加等级和属性字段
    public string level;
    public string attributeA;  // 属性A对应的词条池ID
    public string attributeB;  // 属性B对应的词条池ID
    public string attributeC;  // 属性C对应的词条池ID

    public DragonData(
        int id, 
        string itemName, 
        string icon, 
        string description, 
        string dragonModelAdress, 
        string life, 
        string defense, 
        string attack, 
        string speed,
        string level,
        string attributeA,
        string attributeB,
        string attributeC)
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
        this.level = level;
        this.attributeA = attributeA;
        this.attributeB = attributeB;
        this.attributeC = attributeC;
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
    public float sellPrice;
    public float listPrice;
    public String level;
    
    public SpoilsOfWarData(int id, string itemName, string icon, string description, float sellPrice, float listPrice, string level)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.sellPrice = sellPrice;
        this.listPrice = listPrice;
        this.level = level;
    }
}

[System.Serializable]
public class PropsData
{
    //通用属性
    public int id;
    public string itemName;
    public string icon;
    public string description;

    // 道具特有属性
    public string level;
    public float listPrice;
    public float sellPrice;
    public int itemEffect;     // 道具效果类型
    public float itemEffect1;  // 效果参数1
    public float itemEffect2;  // 效果参数2

    public PropsData(int id, string itemName, string icon, string description, 
        string level, float listPrice, float sellPrice, 
        int itemEffect, float itemEffect1, float itemEffect2)
    {
        this.id = id;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.level = level;
        this.listPrice = listPrice;
        this.sellPrice = sellPrice;
        this.itemEffect = itemEffect;
        this.itemEffect1 = itemEffect1;
        this.itemEffect2 = itemEffect2;
    }
}

[System.Serializable]
public class TermPoolData
{
    // 词条池ID
    public int id;
    
    // 词条池名称
    public string name;
    
    // 词条ID列表字符串 (例如: "1091,1092")
    public string attributePortal;
    
    // 词条概率列表字符串 (例如: "30,10")
    public string attributePro;
    
    public TermPoolData(int id, string name, string attributePortal, string attributePro)
    {
        this.id = id;
        this.name = name;
        this.attributePortal = attributePortal;
        this.attributePro = attributePro;
    }
    
    // 辅助方法：获取词条ID数组
    public string[] GetAttributePortals()
    {
        return attributePortal?.Split(',').Select(s => s.Trim()).ToArray() ?? new string[0];
    }
    
    // 辅助方法：获取概率数组
    public float[] GetAttributeProbabilities()
    {
        return attributePro?.Split(',')
            .Select(s => float.TryParse(s.Trim(), out float result) ? result : 0f)
            .ToArray() ?? new float[0];
    }
}

[System.Serializable]
public class DragonEntryListData
{
    // 词条ID
    public int id;

    // 词条名称
    public string name;

    // 词条描述
    public string description;

    // 词条等级（绿、银、金）
    public string level;

    // 词条效果类型
    public int attributeEffect;

    // 效果参数1-4
    public float attributeEffect1;
    public float attributeEffect2;
    public float attributeEffect3;
    public float attributeEffect4;

    public DragonEntryListData(
        int id,
        string name,
        string description,
        string level,
        int attributeEffect,
        float attributeEffect1,
        float attributeEffect2,
        float attributeEffect3,
        float attributeEffect4)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.level = level;
        this.attributeEffect = attributeEffect;
        this.attributeEffect1 = attributeEffect1;
        this.attributeEffect2 = attributeEffect2;
        this.attributeEffect3 = attributeEffect3;
        this.attributeEffect4 = attributeEffect4;
    }
}

