using UnityEngine;

public class Props : Item
{
    public float sellPrice { get; private set; }
    public float listPrice { get; private set; }
    public string level { get; private set; }
    public int itemEffect { get; private set; }     // 道具效果类型：1为加速孵化，2为获得金币
    public float itemEffect1 { get; private set; }  // 效果参数1：减少的孵化时间
    public float itemEffect2 { get; private set; }  // 效果参数2：获得的金币数量

    public Props(
        string name,
        ItemType type,
        int quantity,
        Sprite icon,
        float sellPrice,
        float listPrice,
        string level,
        int id,
        int itemID,
        string description,
        int itemEffect,
        float itemEffect1,
        float itemEffect2) 
        : base(name, type, quantity, icon, id, itemID, description)
    {
        this.sellPrice = sellPrice;
        this.listPrice = listPrice;
        this.level = level;
        this.itemEffect = itemEffect;
        this.itemEffect1 = itemEffect1;
        this.itemEffect2 = itemEffect2;
    }

    // 使用道具的方法
    public void Use(DragonEgg targetEgg = null)
    {
        switch (itemEffect)
        {
            case 1: // 加速孵化
                if (targetEgg != null)
                {
                    targetEgg.ReduceHatchTime(itemEffect1);
                }
                break;
                
            case 2: // 获得金币
                PlayerDataManager.Instance.AddCoins((int)itemEffect2);
                break;
        }
        
        // 使用后减少数量
        RemoveQuantity(1);
        
        // 如果数量为0，从背包中移除
        if (quantity <= 0)
        {
            Inventory.Instance.RemoveItem(this);
        }
    }
}