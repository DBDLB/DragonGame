using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

    // 添加拖拽相关接口
    public class BagButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
                             IBeginDragHandler, IDragHandler, IEndDragHandler
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
        }
        
        private void OnEnable()
        {
            // 注册按钮的点击事件
            GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
        }
    
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