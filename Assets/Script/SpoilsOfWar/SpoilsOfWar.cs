using UnityEngine;

[System.Serializable]
public class SpoilsOfWar : Item
{
    // 战利品特有属性
    public float sellPrice;   // 出售价格
    public float listPrice;   // 挂牌价格
    public string level;      // 战利品等级

    public SpoilsOfWar(string name, ItemType type, int quantity, Sprite icon, float sellPrice, float listPrice, string level, int id, int itemID, string description) 
        : base(name, type, quantity, icon, id, itemID, description)
    {
        this.sellPrice = sellPrice;
        this.listPrice = listPrice;
        this.level = level;
    }
}