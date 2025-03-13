using UnityEngine;

[System.Serializable]
public class Dragon : Item
{
    //生命
    public int health;
    //攻击力
    public int attack;
    //速度
    public int speed;
    //防御力
    public int defense;
    public Dragon(string name, ItemType type, int quantity, Sprite icon, bool stackable, int health, int attack, int defense,int speed,int id,int itemID)  : base(name, type, quantity, icon, stackable,id,itemID)
    {
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
    }

}