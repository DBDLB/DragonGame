using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // 绑定到背包系统
    public Transform itemSlotContainer; // UI 中用于显示物品的容器
    public GameObject itemSlotPrefab; // 物品槽的预制体

    // 更新背包 UI
    public void UpdateInventoryUI()
    {
        // 清空当前 UI 中的物品槽
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // 为每个物品生成 UI 显示
        foreach (Item item in inventory.items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemSlotContainer);
            slot.GetComponent<ItemSlot>().Initialize(item);  // 将物品绑定到槽位
        }
    }
}