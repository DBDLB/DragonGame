using UnityEngine;
        using UnityEngine.UI;
        using UnityEngine.EventSystems;
        using System.Collections;
        using TMPro;

        public class TooltipManager : MonoBehaviour
        {
            public static TooltipManager Instance;
        
            public GameObject tooltipPanel;
            public GameObject sellItemBotton;
            public GameObject useGamePropsBotton;

            
            
            private bool tooltipActive = false;
            private bool isPointerOverTooltip = false;
            private Transform buttonTransform;
            public Coroutine hideCoroutine = null; // 用于管理隐藏的协程
            private Item currentItem = null; // 当前显示的物品
            private float sellPrice;
        
            private TextMeshProUGUI text;
            private void Awake()
            {
                Instance = this;
                tooltipPanel.SetActive(false);
        
                // 给悬浮框添加事件触发组件
                if (!tooltipPanel.GetComponent<EventTrigger>())
                {
                    EventTrigger trigger = tooltipPanel.AddComponent<EventTrigger>();
        
                    // 添加鼠标进入事件
                    EventTrigger.Entry entryEnter = new EventTrigger.Entry();
                    entryEnter.eventID = EventTriggerType.PointerEnter;
                    entryEnter.callback.AddListener((data) => { 
                        isPointerOverTooltip = true;
                        // 停止任何正在进行的隐藏计时
                        if (hideCoroutine != null)
                        {
                            StopCoroutine(hideCoroutine);
                            hideCoroutine = null;
                        }
                    });
                    trigger.triggers.Add(entryEnter);
        
                    // 添加鼠标离开事件
                    EventTrigger.Entry entryExit = new EventTrigger.Entry();
                    entryExit.eventID = EventTriggerType.PointerExit;
                    entryExit.callback.AddListener((data) => {
                        isPointerOverTooltip = false;
                        // 直接启动隐藏协程
                        hideCoroutine = StartCoroutine(DelayHideTooltip());
                    });
                    trigger.triggers.Add(entryExit);
                    
                    text = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        
            private void Update()
            {
                if (tooltipPanel.activeSelf && tooltipActive)
                {
                    Vector3 offset = new Vector3((buttonTransform.GetComponent<RectTransform>().sizeDelta).x / 2 * buttonTransform.GetComponent<RectTransform>().localScale.x, 0, 0);
                    tooltipPanel.transform.position = buttonTransform.position - (Vector3)tooltipPanel.GetComponent<RectTransform>().sizeDelta / 2 - offset;
                    tooltipActive = false;
                }
            }
            
            public void ShowTooltip(Item item, Transform position)
            {
                // 停止任何正在进行的隐藏计时
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                    hideCoroutine = null;
                }
        
                // 更新当前项
                currentItem = item;
                tooltipPanel.SetActive(true);
                tooltipActive = true;
                this.buttonTransform = position;
                
                //显示价格
                switch (item.itemType)
                {
                    case ItemType.DragonEgg:
                        sellPrice = (item as DragonEgg).sellPrice;
                        text.text = "价格：" + sellPrice;
                        break;
                    case ItemType.Dragon:
                        Dragon dragon = item as Dragon;
                        text.text = "生命值：" + dragon.life + "\n" + "攻击力：" + dragon.attack + "\n" + "防御力：" + dragon.defense + "\n" + "速度：" + dragon.speed;
                        
                        break;
                }
            }
        
            public void HideTooltip()
            {
                // 只有当鼠标不在悬浮框上时才隐藏
                if (!isPointerOverTooltip)
                {
                    tooltipPanel.SetActive(false);
                    currentItem = null;
                }
            }
        
            // 延迟隐藏协程集中在TooltipManager中
            public IEnumerator DelayHideTooltip()
            {
                yield return new WaitForSeconds(0.1f);
                HideTooltip();
                hideCoroutine = null;
            }
        
            // 检查鼠标是否在悬浮框上的公共方法
            public bool IsPointerOverTooltip()
            {
                return isPointerOverTooltip;
            }
            
            // 售卖物品
            public void SellItem()
            {
                //增加金币
                PlayerDataManager.Instance.AddCoin(sellPrice);
                ShopManager.Instance.ShowCoin();
                //从背包中移除物品
                Inventory.Instance.RemoveItem(currentItem);
                tooltipPanel.SetActive(false);
                currentItem = null;
            }
        }