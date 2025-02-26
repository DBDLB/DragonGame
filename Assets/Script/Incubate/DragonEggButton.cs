using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonEggButton : MonoBehaviour
{
    public DragonEgg item;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        IncubationUI.Instance.ShowEggDetails(item);
        Image image = IncubationUI.Instance.selectEgg;
        image.sprite = item.icon;
        image.color = Color.white;
        IncubationUI.Instance.startButton.interactable = true;
    }
}
