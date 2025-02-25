using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image itemIcon; // 物品图标
    public Text quantityText; // 物品数量文本
    private Item item; // 当前槽位所对应的物品

    // 初始化物品槽
    public void Initialize(Item item)
    {
        this.item = item;
        itemIcon.sprite = item.icon; // 设置图标
        quantityText.text = item.quantity.ToString(); // 显示数量
    }

    // 点击物品槽时调用的函数（例如查看物品、使用物品等）
    public void OnItemClick()
    {
        Debug.Log("点击了物品: " + item.itemName);
        // 可以根据物品类型执行不同的操作，例如孵化龙蛋
        if (item.itemType == ItemType.DragonEgg)
        {
            // 触发孵蛋操作
            // 例如：打开孵蛋界面并显示该龙蛋的详细信息
        }
    }
}