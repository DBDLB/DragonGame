using UnityEngine;

public class Dragon : Item
{
    //生命
    public int health;
    //攻击力
    public int attack;
    //防御力
    public int defense;
    public Dragon(string name, ItemType type, int quantity, Sprite icon, bool stackable, int health, int attack, int defense) : base(name, type, quantity, icon, stackable)
    {
        this.health = health;
        this.attack = attack;
        this.defense = defense;
    }

}