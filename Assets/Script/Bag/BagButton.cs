using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BagButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(item,this.transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 延迟调用HideTooltip，给鼠标一个移动到悬浮框的机会
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

    }
}
