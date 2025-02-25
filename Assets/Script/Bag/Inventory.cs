using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 背包中可以存储的物品数量
    public int maxSlots = 100;
    public List<Item> items = new List<Item>(); // 存储所有物品的列表
    
    
    public static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventory>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(Inventory).Name);
                    instance = singleton.AddComponent<Inventory>();
                }
            }
            return instance;
        }
    }

    // 添加物品到背包
    public bool AddItem(Item item)
    {
        // 如果背包没有空位
        if (items.Count >= maxSlots) return false;

        // 如果物品可以堆叠且背包已有该物品
        if (item.isStackable)
        {
            foreach (var existingItem in items)
            {
                if (existingItem.itemName == item.itemName)
                {
                    existingItem.AddQuantity(item.quantity);
                    return true;
                }
            }
        }

        // 否则，直接添加新的物品
        items.Add(item);
        return true;
    }

    // 移除物品
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    // 获取物品的数量
    public int GetItemCount(Item item)
    {
        return items.Find(i => i.itemName == item.itemName)?.quantity ?? 0;
    }

    // 清空背包
    public void Clear()
    {
        items.Clear();
    }
}
