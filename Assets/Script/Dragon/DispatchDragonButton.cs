using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DispatchDragonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Dragon item;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(item, this.transform);
        TooltipManager.Instance.sellItemBotton.SetActive(false);
        TooltipManager.Instance.useGamePropsBotton.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 直接通知TooltipManager处理隐藏逻辑
        TooltipManager.Instance.hideCoroutine = TooltipManager.Instance.StartCoroutine(TooltipManager.Instance.DelayHideTooltip());
    }

    private IEnumerator DelayHideTooltip()
    {
        // 等待短暂时间，让鼠标有机会移到悬浮框上
        yield return new WaitForSeconds(0.1f);
        TooltipManager.Instance.HideTooltip();
    }
    
    private void OnClicked()
    {
        // ShowDragonAttribute.Instance.ShowDragonUI(item);
        DispatchManager.Instance.selectedDragon = item;
        DispatchManager.Instance.dragonImage.sprite = item.icon;
        DispatchManager.Instance.dragonImage.color = Color.white;
        // DispatchManager.Instance.dragonImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "龙出行中";
    }
}
