using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// 添加拖拽相关接口
    public class BagButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                         IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public Item item;
        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private Vector3 originalPosition;
        private Transform originalParent;
        private GameObject dragItemInstance;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    
            // 确保Image组件的raycastTarget为true
            Image image = GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }
        }
        
        private void OnEnable()
        {
            // 注册按钮的点击事件
            // GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
        }
        
        // 实现右键点击接口
        public void OnPointerClick(PointerEventData eventData)
        {
            // 左键点击处理
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClicked();
            }
            // 右键点击处理
            else if (eventData.button == PointerEventData.InputButton.Right && item != null)
            {
                Debug.Log("右键点击 - 尝试上架物品");
                TryPlaceItemOnShelf();
            }
        }
        
        // 尝试将物品放置到货架上
    private void TryPlaceItemOnShelf()
    {
        if (item == null) return;

        // 获取所有货架槽位
        List<ShelfSlot> availableSlots = new List<ShelfSlot>();
        
        // 检查ShelfManager是否存在
        if (ShelfManager.Instance != null && ShelfManager.Instance.shelfSlots != null)
        {
            // 寻找空的货架槽位
            foreach (ShelfSlot slot in ShelfManager.Instance.shelfSlots)
            {
                if (slot != null && slot.GetCurrentItem() == null)
                {
                    availableSlots.Add(slot);
                }
            }

            // 如果找到可用槽位，将物品放入第一个空槽位
            if (availableSlots.Count > 0)
            {
                // 尝试放置物品
                bool placed = availableSlots[0].PlaceItem(item);
                
                if (placed)
                {
                    // 从背包移除物品
                    Inventory.Instance.RemoveItem(item);
                    
                    // 更新背包UI
                    transform.parent.GetComponentInParent<ShowBagUI>().ShowBag();
                    
                    // 可以添加一个简单的反馈效果
                    // StartCoroutine(ShowPlacementFeedback());
                }
            }
            else
            {
                // 没有可用槽位时显示提示
                Debug.Log("没有可用的货架槽位");
                // 可以添加UI提示
            }
        }
    }

    // 显示放置成功的反馈效果
    // private IEnumerator ShowPlacementFeedback()
    // {
    //     // 创建一个临时UI元素显示"已上架"提示
    //     GameObject feedbackObj = new GameObject("PlacementFeedback");
    //     feedbackObj.transform.SetParent(canvas.transform);
    //     
    //     TextMeshProUGUI feedbackText = feedbackObj.AddComponent<TextMeshProUGUI>();
    //     feedbackText.text = "已上架";
    //     feedbackText.fontSize = 24;
    //     feedbackText.color = Color.green;
    //     
    //     RectTransform rect = feedbackObj.GetComponent<RectTransform>();
    //     rect.position = transform.position;
    //     rect.sizeDelta = new Vector2(100, 30);
    //     
    //     // 淡出效果
    //     float duration = 1.0f;
    //     float startTime = Time.time;
    //     
    //     while (Time.time < startTime + duration)
    //     {
    //         float alpha = 1 - ((Time.time - startTime) / duration);
    //         feedbackText.color = new Color(feedbackText.color.r, feedbackText.color.g, feedbackText.color.b, alpha);
    //         rect.position += Vector3.up * Time.deltaTime * 50; // 向上飘动
    //         yield return null;
    //     }
    //     
    //     Destroy(feedbackObj);
    // }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.Instance.ShowTooltip(item, this.transform);
            TooltipManager.Instance.sellItemBotton.SetActive(true);
            if (item.itemType == ItemType.GameProps)
            {
                TooltipManager.Instance.useGamePropsBotton.SetActive(true);
            }
            else
            {
                TooltipManager.Instance.useGamePropsBotton.SetActive(false);
            }
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.hideCoroutine = TooltipManager.Instance.StartCoroutine(TooltipManager.Instance.DelayHideTooltip());
        }
        
        // 开始拖拽
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item == null) return; // 没有物品不能拖拽
            
            originalPosition = rectTransform.position;
            originalParent = transform.parent;
            
            // 创建拖拽时显示的物品实例
            dragItemInstance = new GameObject("DragItem");
            dragItemInstance.transform.SetParent(canvas.transform);
            Image dragImage = dragItemInstance.AddComponent<Image>();
            dragImage.sprite = item.icon;
            dragImage.raycastTarget = false; // 避免拖拽图像阻挡射线检测
            
            // 设置拖拽图像大小和位置
            RectTransform dragRect = dragItemInstance.GetComponent<RectTransform>();
            dragRect.sizeDelta = rectTransform.sizeDelta;
            dragRect.position = eventData.position;
            
            // 修改原按钮的透明度
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false; // 允许射线穿过，以便检测下方物体
        }
        
        // 拖拽中
        public void OnDrag(PointerEventData eventData)
        {
            if (dragItemInstance != null)
            {
                dragItemInstance.GetComponent<RectTransform>().position = eventData.position;
            }
        }
        
        // 拖拽结束
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            
            bool itemPlaced = false;
            // 获取所有被射线击中的UI元素
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            
            // 遍历所有结果，查找ShelfSlot
            foreach (var result in results)
            {
                ShelfSlot shelfSlot = result.gameObject.GetComponent<ShelfSlot>();
                if (shelfSlot != null)
                {
                    // 找到ShelfSlot，尝试放置物品
                    itemPlaced = shelfSlot.PlaceItem(item);

                    if (itemPlaced)
                    {
                        // 如果成功放置，从背包中移除物品
                        Inventory.Instance.RemoveItem(item);
                        // 更新背包UI
                        transform.parent.GetComponentInParent<ShowBagUI>().ShowBag();
                        break; // 找到并放置后跳出循环
                    }
                }
            }
            
            // 如果未成功放置，返回原位
            if (!itemPlaced)
            {
                ReturnToOriginalPosition();
            }
            
            // 销毁拖拽时创建的实例
            if (dragItemInstance != null)
            {
                Destroy(dragItemInstance);
            }
        }
        
        private void ReturnToOriginalPosition()
        {
            transform.position = originalPosition;
        }
        
        private void OnClicked()
        {
            // 保持原有点击功能
        }
    }