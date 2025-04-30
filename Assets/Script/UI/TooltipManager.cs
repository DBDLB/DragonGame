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
                    RectTransform buttonRect = buttonTransform.GetComponent<RectTransform>();
                    RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

                    // 获取按钮的世界空间尺寸（考虑缩放）
                    Vector3[] buttonCorners = new Vector3[4];
                    buttonRect.GetWorldCorners(buttonCorners);
                    float buttonWidth = Vector3.Distance(buttonCorners[0], buttonCorners[3]);
                    float buttonHeight = Vector3.Distance(buttonCorners[0], buttonCorners[1]);

                    // 获取tooltip的世界空间尺寸
                    Vector3[] tooltipCorners = new Vector3[4];
                    tooltipRect.GetWorldCorners(tooltipCorners);
                    float tooltipWidth = Vector3.Distance(tooltipCorners[0], tooltipCorners[3]);
                    float tooltipHeight = Vector3.Distance(tooltipCorners[0], tooltipCorners[1]);

                    // 获取按钮在屏幕上的位置
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(buttonTransform.position);

                    // 设置基础偏移量
                    float padding = 5f;

                    // 计算新位置（考虑缩放）
                    Vector3 newPosition = buttonTransform.position;

                    // 水平位置调整
                    if (screenPoint.x + tooltipWidth + padding < Screen.width)
                    {
                        // 显示在右侧
                        newPosition.x += (buttonWidth / 2 + tooltipWidth / 2 + padding);
                    }
                    else
                    {
                        // 显示在左侧
                        newPosition.x -= (buttonWidth / 2 + tooltipWidth / 2 + padding);
                    }

                    // 垂直位置调整
                    if (screenPoint.y + tooltipHeight/2 > Screen.height)
                    {
                        // 如果tooltip会超出屏幕上方，向下调整
                        newPosition.y -= (tooltipHeight - buttonHeight) / 1.2f;
                    }
                    else if (screenPoint.y - tooltipHeight/2 < 0)
                    {
                        // 如果tooltip会超出屏幕下方，向上调整
                        newPosition.y += (tooltipHeight - buttonHeight) / 1.2f;
                    }

                    tooltipPanel.transform.position = newPosition;
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
                    case ItemType.Props:
                        Props props = item as Props;
                        sellPrice = props.sellPrice;
                        text.text = "价格：" + sellPrice + "\n" + "效果：" + props.description;
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
                PlayerDataManager.Instance.AddCoins(sellPrice);
                // ShopManager.Instance.ShowCoin();
                //从背包中移除物品
                Inventory.Instance.RemoveItem(currentItem);
                tooltipPanel.SetActive(false);
                currentItem = null;
            }
            
            public void UseGameProps()
            {
                if (currentItem != null && currentItem.itemType == ItemType.Props)
                {
                    // 使用道具
                    (currentItem as Props).Use();
                    tooltipPanel.SetActive(false);
                    currentItem = null;
                }
            }
        }