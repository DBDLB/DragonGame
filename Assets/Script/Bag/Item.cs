using System;
using UnityEngine;

public enum ItemType
{
    DragonEgg,
    Resource,
    Equipment
}

[System.Serializable]
public class Item:MonoBehaviour
{
    public string itemName;   // 物品名称
    public ItemType itemType; // 物品类型
    public int quantity;      // 物品数量
    public Sprite icon;       // 物品图标
    public bool isStackable;  // 是否可以堆叠

    public Item(string name, ItemType type, int quantity, Sprite icon, bool stackable)
    {
        itemName = name;
        itemType = type;
        this.quantity = quantity;
        this.icon = icon;
        isStackable = stackable;
    }
    public Item()
    {
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
}