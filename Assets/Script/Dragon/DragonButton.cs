using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Dragon item;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
    
    private void OnClicked()
    {
        // ShowDragonAttribute.Instance.ShowDragonUI(item);
        DispatchManager.Instance.selectedDragon = item;
        DispatchManager.Instance.dragonImage.sprite = item.icon;
        DispatchManager.Instance.dragonImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "龙出行中";
    }
}
