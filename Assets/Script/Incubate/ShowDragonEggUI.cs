using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDragonEggUI : MonoBehaviour
{
    public static EggIncubatorController eggIncubatorController;
    public GameObject DragonEggButton;
    public GameObject Content;

    private void OnEnable()
    {
        foreach (var item in Inventory.Instance.items)
        {
            if (item.itemType == ItemType.DragonEgg)
            {
                //生成一个DragonEggButton到Content下
                GameObject slot = Instantiate(DragonEggButton, Content.transform);
                slot.GetComponent<DragonEggButton>().item = item as DragonEgg;
                Image image = null;
                Image[] images = slot.GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    if (img.gameObject != slot)
                    {
                        image = img;
                    }
                }
                image.sprite = item.icon;
            }
        }
    }
}
