using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBagUI : MonoBehaviour
{
    public static EggIncubatorController BagIncubatorController;
    public GameObject ShowBagButton;
    public GameObject Content;

    private void OnEnable()
    {
        foreach (var item in Inventory.Instance.items)
        {
            //生成一个DragonEggButton到Content下
            GameObject slot = Instantiate(ShowBagButton, Content.transform);
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
