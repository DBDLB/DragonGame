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
    public Dragon(string name, ItemType type, int quantity, Sprite icon, int life, int attack, int defense,int speed,int id,int itemID,string description,string dragonModelAdress)  : base(name, type, quantity, icon,id,itemID,description)
    {
        this.life = life;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.dragonModelAdress = dragonModelAdress;
    }

}