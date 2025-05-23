using TMPro;
using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    
    public class ShelfSlot : MonoBehaviour, IDropHandler
    {
        public Image itemImage;
        public TextMeshProUGUI priceText;
        private Item currentItem;
        private int price;
        
        public Item GetCurrentItem()
        {
            return currentItem;
        }

        public int GetPrice()
        {
            return price;
        }
        
        // 接收拖放的物品
        public void OnDrop(PointerEventData eventData)
        {
            if (currentItem != null) return; // 已有物品则不接受新物品
            
            BagButton bagButton = eventData.pointerDrag.GetComponent<BagButton>();
            if (bagButton != null && bagButton.item != null)
            {
                // 此处只进行检测，实际放置通过PlaceItem方法完成
            }
        }
        
        // 放置物品到货架，返回是否成功
        public bool PlaceItem(Item item)
        {
            if (currentItem != null) return false;
            
            itemImage.color = Color.white;
            currentItem = item;
            itemImage.sprite = item.icon;
            itemImage.enabled = true;
            
            // 设置物品价格
            SetPrice(CalculateDefaultPrice(item));
            ShelfManager.Instance.SaveShelf();
            
            return true;
        }
        
        // 设置物品价格
        public void SetPrice(int newPrice)
        {
            price = newPrice;
            if (priceText != null)
            {
                priceText.text = price.ToString();
            }
        }
        
        // 从货架移除物品
        public Item RemoveItem()
        {
            Item removedItem = currentItem;
            currentItem = null;
            itemImage.sprite = null;
            itemImage.color = new Color(1, 1, 1, 0);
            itemImage.enabled = false;
            priceText.text = "";
            ShelfManager.Instance.SaveShelf();
            return removedItem;
        }
        
        // 计算默认价格
        private int CalculateDefaultPrice(Item item)
        {
            // 根据物品类型设置不同价格
            switch (item.itemType)
            {
                case ItemType.DragonEgg:
                    DragonEgg egg = item as DragonEgg;
                    return Mathf.RoundToInt(egg.listPrice);
                case ItemType.Dragon:
                    return 1000;
                case ItemType.Props:
                    Props props = item as Props;
                    return Mathf.RoundToInt(props.listPrice);
                default:
                    return 100;
            }
        }
    }