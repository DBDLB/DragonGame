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
    private HashSet<int> discoveredItems = new HashSet<int>();
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/playerData.json";
        LoadDiscoveredItems();
    }

    // 检查物品是否为首次获得
    public bool IsNewDiscovery(int itemId)
    {
        return !discoveredItems.Contains(itemId);
    }

    // 记录新获得的物品种类
    public void MarkItemDiscovered(int itemId)
    {
        bool isNew = !discoveredItems.Contains(itemId);
        discoveredItems.Add(itemId);
        SaveDiscoveredItems();

        if (isNew)
        {
            Debug.Log("首次获得物品种类：" + itemId);
            // 这里可以触发首次获得物品的事件
        }
    }

    // 保存已发现物品列表
    private void SaveDiscoveredItems()
    {
        PlayerData data = new PlayerData();
        data.discoveredItemIds = new List<int>(discoveredItems).ToArray();
        
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
            
            discoveredItems = new HashSet<int>(data.discoveredItemIds);
        }
    }

    [System.Serializable]
    private class PlayerData
    {
        public int[] discoveredItemIds;
    }
}