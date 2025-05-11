using UnityEngine;

[System.Serializable]
public class Dragon : Item
{
    //龙模型地址
    public string dragonModelAdress;
    //生命
    public int life;
    //攻击力
    public int attack;
    //速度
    public int speed;
    //防御力
    public int defense;
    
    //等级
    public string level;
    
    //属性
    public string attributeA;
    public string attributeB;
    public string attributeC;

    public Dragon(
        string name, 
        ItemType type, 
        int quantity, 
        Sprite icon, 
        int life, 
        int attack, 
        int defense, 
        int speed,
        int id,
        int itemID,
        string description,
        string dragonModelAdress,
        string level,
        string attributeA,
        string attributeB,
        string attributeC) 
        : base(name, type, quantity, icon, id, itemID, description)
    {
        this.life = life;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.dragonModelAdress = dragonModelAdress;
        this.level = level;
        this.attributeA = attributeA;
        this.attributeB = attributeB;
        this.attributeC = attributeC;
    }

}