using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUI inventoryUI;

    private void Start()
    {
        // 初始化时更新 UI
        inventoryUI.UpdateInventoryUI();
    }

    // 处理物品使用（例如孵化龙蛋）
    public void UseItem(Item item)
    {
        if (item.itemType == ItemType.DragonEgg)
        {
            // 触发孵化过程
            Debug.Log("孵化龙蛋: " + item.itemName);
            // 将物品数量减少
            item.RemoveQuantity(1);

            // 在 UI 上更新背包
            inventoryUI.UpdateInventoryUI();
        }
    }
}