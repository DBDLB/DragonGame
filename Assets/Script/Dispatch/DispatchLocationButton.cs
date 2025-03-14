using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispatchLocationButton : MonoBehaviour
{
    public int locationID;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        // ShowDragonAttribute.Instance.ShowDragonUI(item);
        DispatchManager.Instance.locationID = locationID;
        DispatchManager.Instance.locationImage.sprite = GetComponentInChildren<Image>().sprite;
    }
}
