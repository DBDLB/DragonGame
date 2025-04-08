using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(PlayerDataManager).Name);
                    instance = singleton.AddComponent<PlayerDataManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }

    // 玩家已获得过的物品种类ID列表(使用id而非itemID，因为id代表物品种类)
    // 新增存储物品类型和ID的集合
    private List<ItemDiscoveryInfo> itemDiscoveryList = new List<ItemDiscoveryInfo>();
    private string saveFilePath;
    
    //玩家金币数
    public float coin = 0;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/playerData.json";
        LoadDiscoveredItems();
    }

    // 检查物品是否为首次获得
    public bool IsNewDiscovery(int id, ItemType itemType)
    {
        return !itemDiscoveryList.Exists(item => item.id == id && item.itemType == itemType.ToString());
    }

    // 记录新获得的物品种类
    public void MarkItemDiscovered(int id, ItemType itemType)
    {
        
        if (!IsNewDiscovery(id, itemType))
            return;
        
        ItemDiscoveryInfo newItem = new ItemDiscoveryInfo
        {
            id = id,
            itemType = itemType.ToString()
        };
    
        itemDiscoveryList.Add(newItem);
        SavePlayerData();
    
        Debug.Log($"首次获得物品：ID={id}，类型={itemType}");
    }
    
    // 保存已发现物品列表
    private void SavePlayerData()
    {
        PlayerData data = new PlayerData
        {
            discoveredItems = itemDiscoveryList.ToArray(),
            coin = coin // 保存金币数量
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }
    
    //增加金币
    public void AddCoin(float amount)
    {
        coin += amount;
        SavePlayerData();
    }

    // 加载已发现物品列表
    private void LoadDiscoveredItems()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            if (data.discoveredItems != null)
                itemDiscoveryList = new List<ItemDiscoveryInfo>(data.discoveredItems);
            coin = data.coin; // 加载金币数
        }
    }

    [System.Serializable]
    private class PlayerData
    {
        public ItemDiscoveryInfo[] discoveredItems;
        public float coin; // 添加金币字段
    }
    
    [System.Serializable]
    private class ItemDiscoveryInfo
    {
        public string itemType; // 使用字符串存储ItemType枚举值
        public int id;
    }
}