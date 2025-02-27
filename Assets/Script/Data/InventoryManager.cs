using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;  // 玩家背包

    private string filePath;

    private void Start()
    {
        filePath = Application.persistentDataPath + "/inventory.json";  // 保存路径
        LoadInventory();  // 启动时加载物品
    }

    // 保存背包数据到 JSON 文件
    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(inventory, true);  // 将背包对象序列化为 JSON 字符串
        File.WriteAllText(filePath, json);  // 写入文件
        Debug.Log("Inventory saved to: " + filePath);
    }

    // 从 JSON 文件加载背包数据
    public void LoadInventory()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);  // 读取 JSON 字符串
            inventory = JsonUtility.FromJson<Inventory>(json);  // 反序列化 JSON 字符串为 Inventory 对象
            Debug.Log("Inventory loaded.");
        }
        else
        {
            Debug.Log("Inventory file not found, creating new.");
            inventory = new Inventory();  // 如果文件不存在，初始化一个新的背包
        }
    }

    // 玩家获得物品并保存
    public void AddItemToInventory(Item item)
    {
        inventory.items.Add(item);
        SaveInventory();  // 每次修改物品后保存
    }
}