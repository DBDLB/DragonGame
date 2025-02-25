using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(UIManager).Name);
                    instance = singleton.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }
    
    public GameObject[] panels; // 存储所有面板的数组
    // 需要关闭的 UI 面板
    public GameObject[] maskCloseUiPanels;
    // 遮罩层
    public GameObject mask;
    
    private void Start()
    {
        // 初始化时隐藏所有面板
        // HideAllPanels();
    }

    // 显示指定面板
    public void ShowPanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(true);
                if (HasPanelInMaskCloseUiPanels())
                {
                    mask.SetActive(true); // 显示遮罩层
                }
                break;
            }
        }
    }
    
    public void HidePanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(false);
                break;
            }
        }
    }

    // 隐藏所有面板
    public void HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    // 切换面板显示状态
    public void TogglePanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(!panel.activeSelf);
                break;
            }
        }
    }

    string FormatMoney(long money)
    {
        if (money >= 1000000000)
        {
            return (money / 1000000000f).ToString("F1") + "B"; // 十亿
        }
        else if (money >= 1000000)
        {
            return (money / 1000000f).ToString("F1") + "M"; // 百万
        }
        else if (money >= 1000)
        {
            return (money / 1000f).ToString("F1") + "K"; // 千
        }
        else
        {
            return money.ToString("N0"); // 小于千的正常显示
        }
    }
    
    // 关闭 UI 面板
    public void CloseUI()
    {
        foreach (var uiPanel in maskCloseUiPanels)
        {
            if (uiPanel.activeSelf)
            {
                uiPanel.SetActive(false);
                mask.SetActive(false);  // 同时关闭遮罩层
            }
        }
    }
    
    bool HasPanelInMaskCloseUiPanels()
    {
        foreach (GameObject panel in maskCloseUiPanels)
        {
            if (panel != null)
            {
                return true;
            }
        }
        return false;
    }
    

    // 打开 UI 面板
    // public void OpenUI()
    // {
    //     foreach (var uiPanel in maskCloseUiPanels)
    //     {
    //         uiPanel.SetActive(true);
    //         mask.SetActive(true); // 显示遮罩层
    //     }
    // }
}