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
    public List<GameObject> slots = new List<GameObject>();

    private void OnEnable()
    {
        ShowBag();
    }

    public void ShowBag()
    {
        //删除Content下的所有子物体
        foreach (var slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
        
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
            slots.Add(slot);
        }

        if (slots.Count < Inventory.Instance.maxSlots)
        {
            for (int i = slots.Count; i < Inventory.Instance.maxSlots; i++)
            {
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
                image.sprite = null;
                slots.Add(slot);
            }
        }

    }
}
