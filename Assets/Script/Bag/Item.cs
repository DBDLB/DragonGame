using UnityEngine;

public enum ItemType
{
    DragonEgg,//龙蛋
    SpoilsOfWar,//战利品
    Dragon, //龙
    Props, //道具
}

[System.Serializable]
public class Item
{
    public string itemName;       // 物品名称
    public ItemType itemType;     // 物品类型
    public int quantity;          // 物品数量
    public Sprite icon;           // 物品图标
    public int itemID;           // 物品ID（唯一标识）
    public int id;                // 物品种类ID
    public string description;    // 物品描述

    public Item(string name, ItemType type, int quantity, Sprite icon,int id,int itemID, string description)
    {
        itemName = name;
        itemType = type;
        this.quantity = quantity;
        this.icon = icon;
        this.itemID = itemID;
        this.id = id;
        this.description = description;
    }

    // 增加物品数量
    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    // 减少物品数量
    public void RemoveQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity - amount);
    }
    
    
    public static class ItemIDGenerator
    {
        private static int currentID;

        static ItemIDGenerator()
        {
            // 尝试从 PlayerPrefs 获取上次保存的 ID
            currentID = PlayerPrefs.GetInt("LastItemID", 0);
        }

        // 获取下一个唯一 ID
        public static int GetUniqueID()
        {
            // 当前 ID 递增
            int newID = currentID++;

            // 将新的 ID 保存到 PlayerPrefs 中
            PlayerPrefs.SetInt("LastItemID", currentID);

            // 强制保存到磁盘
            PlayerPrefs.Save();

            return newID;
        }
    }
    
}