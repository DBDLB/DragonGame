using System;
using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    
    public class NewItemPopup : MonoBehaviour
    {
        public static NewItemPopup instance;
        public static NewItemPopup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<NewItemPopup>();
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(NewItemPopup).Name);
                        instance = singleton.AddComponent<NewItemPopup>();
                    }
                }
                return instance;
            }
        }
        
    
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        // [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private Button closeButton;
        
        public List<Item> items = new List<Item>();
    
        private void Awake()
        {
            // 默认隐藏弹窗
            popupPanel.SetActive(false);
            
            // 添加关闭按钮事件
            closeButton.onClick.AddListener(ClosePopup);
        }
        
        private bool isShowing = false;
        public void ShowPopup()
        {
            if (items.Count > 0 && !isShowing)
            {
                ShowNewItemPopup(items[0]);
                items.RemoveAt(0);
                isShowing = true;
            }
        }

        // 显示新物品弹窗
        public void ShowNewItemPopup(Item item)
        {
            // 设置物品信息
            itemIcon.sprite = item.icon;
            itemNameText.text = "新！ " +  item.itemName;
            
            // 根据物品类型获取描述
            switch (item.itemType)
            {
                case ItemType.DragonEgg:
                    DragonEgg egg = item as DragonEgg;
                    // itemDescriptionText.text = egg.description;
                    break;
                case ItemType.Dragon:
                    Dragon dragon = item as Dragon;
                    // itemDescriptionText.text = dragon.description;
                    break;
                case ItemType.SpoilsOfWar:
                    // 处理战利品描述
                    break;
            }
            
            // 显示弹窗
            popupPanel.SetActive(true);
            
            // 可选：播放动画或音效
            // GetComponent<Animation>().Play("PopupAnimation");
        }
    
        // 关闭弹窗
        public void ClosePopup()
        {
            popupPanel.SetActive(false);
            if (items.Count > 0)
            {
                ShowNewItemPopup(items[0]);
                items.RemoveAt(0);
            }
            else
            {
                isShowing = false;
            }
        }
    }