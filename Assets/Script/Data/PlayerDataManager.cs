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

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/playerData.json";
        LoadDiscoveredItems();
    }

    // 检查物品是否为首次获得
    public bool IsNewDiscovery(int itemId, ItemType itemType)
    {
        return !itemDiscoveryList.Exists(item => item.itemId == itemId && item.itemType == itemType.ToString());
    }

    // 记录新获得的物品种类
    public void MarkItemDiscovered(int itemId, ItemType itemType)
    {
        
        if (!IsNewDiscovery(itemId, itemType))
            return;
        
        ItemDiscoveryInfo newItem = new ItemDiscoveryInfo
        {
            itemId = itemId,
            itemType = itemType.ToString()
        };
    
        itemDiscoveryList.Add(newItem);
        SaveDiscoveredItems();
    
        Debug.Log($"首次获得物品：ID={itemId}，类型={itemType}");
    }
    
    // 保存已发现物品列表
    private void SaveDiscoveredItems()
    {
        PlayerData data = new PlayerData
        {
            discoveredItems = itemDiscoveryList.ToArray()
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
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
        }
    }

    [System.Serializable]
    private class PlayerData
    {
        public ItemDiscoveryInfo[] discoveredItems;
    }
    
    [System.Serializable]
    private class ItemDiscoveryInfo
    {
        public string itemType; // 使用字符串存储ItemType枚举值
        public int itemId;
    }
}