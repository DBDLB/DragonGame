using UnityEngine;

[System.Serializable]
public class Dragon : Item
{
    //龙模型地址
    public string dragonModelAdress;
    //生命
    public int health;
    //攻击力
    public int attack;
    //速度
    public int speed;
    //防御力
    public int defense;
    public Dragon(string name, ItemType type, int quantity, Sprite icon, int health, int attack, int defense,int speed,int id,int itemID,string description,string dragonModelAdress)  : base(name, type, quantity, icon,id,itemID,description)
    {
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.dragonModelAdress = dragonModelAdress;
    }

}