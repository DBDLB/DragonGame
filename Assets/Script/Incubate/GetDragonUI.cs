using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetDragonUI : MonoBehaviour
{
    public Image dragonSprite;
    public TextMeshProUGUI dragonName;
    public TextMeshProUGUI dragonHealth;
    public TextMeshProUGUI dragonAttack;
    public TextMeshProUGUI dragonDefense;
    
    public void ShowDragonUI(Dragon dragon)
    {
        dragonSprite.sprite = dragon.icon;
        dragonName.text = "名称: " + dragon.itemName;
        dragonHealth.text = "生命值: " + dragon.life.ToString();
        dragonAttack.text = "攻击力: " + dragon.attack.ToString();
        dragonDefense.text = "防御力: " + dragon.defense.ToString();
    }
}
