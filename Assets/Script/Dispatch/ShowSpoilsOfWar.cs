using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSpoilsOfWar : MonoBehaviour
{
    public static EggIncubatorController BagIncubatorController;
    public GameObject ShowBagButton;
    public GameObject NormalBagButton;
    public GameObject Content;
    public List<GameObject> slots = new List<GameObject>();
    public List<Item> items = new List<Item>();
    

    private void OnEnable()
    {
        ShowBag(items);
    }

    public void ShowBag(List<Item> items)
    {
        //删除Content下的所有子物体
        foreach (var slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
        
        foreach (var item in items)
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
            slot.GetComponent<BagButton>().item = item;
            slots.Add(slot);
        }
    }
}
