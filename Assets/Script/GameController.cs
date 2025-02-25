using UnityEngine;

public class GameController : MonoBehaviour
{
    public Inventory inventory;
    public ItemController itemController;

    public void OnClickEggButton()
    {
        // 示例：玩家点击孵蛋按钮后，选择一个龙蛋开始孵化
        Item dragonEgg = inventory.items.Find(item => item.itemType == ItemType.DragonEgg);
        if (dragonEgg != null)
        {
            itemController.UseItem(dragonEgg);  // 开始孵化龙蛋
        }
    }
}