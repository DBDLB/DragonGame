using System.Collections;
using TMPro;
using UnityEngine;
    
    public class Customer : MonoBehaviour
    {
        public int money;
        private bool hasBought = false;
        
        // 客人进入商店的动画时间
        [SerializeField] private float enterTime = 1.5f;//客人进入商店的时间
        [SerializeField] private float browsingTime = 2f;//客人浏览商品的时间
        [SerializeField] private float purchaseTime = 0.5f;//客人购买商品的时间
        [SerializeField] private float leaveTime = 1.5f;//客人离开商店的时间
        
        // 对外事件，用于动画触发
        public delegate void CustomerAction();
        public event CustomerAction OnEnterShop;//进入商店
        public event CustomerAction OnBrowse;//浏览商品
        public event CustomerAction OnLeave;//离开商店
        
        private TextMeshProUGUI textMeshPro;
        private void Start()
        {
            StartCoroutine(ShoppingProcess());
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = money.ToString();
        }
        
        private IEnumerator ShoppingProcess()
        {
            // 进入商店
            if (OnEnterShop != null) OnEnterShop();
            yield return new WaitForSeconds(enterTime);
        
            // 先浏览商品
            if (OnBrowse != null) OnBrowse();
            yield return new WaitForSeconds(browsingTime);
            
            // 检查是否有可购买的商品
            ShelfSlot availableSlot = FindBestAffordableItem();
            if (availableSlot == null)
            {
                // 如果没有可购买的商品，直接离开
                if (OnLeave != null) OnLeave();
                yield return new WaitForSeconds(leaveTime);
                Destroy(gameObject, 1f);
                yield break;
            }
        
            // 尝试购买商品，最多购买6个
            int purchaseCount = 0;
            while (money > 0 && purchaseCount < 6)
            {
                ShelfSlot bestSlot = FindBestAffordableItem();
                
                if (bestSlot == null)
                    break; // 没有足够钱购买任何商品
        
                // 购买商品
                Item purchasedItem = bestSlot.RemoveItem();
                int itemPrice = bestSlot.GetPrice();
        
                // 更新金钱
                money -= itemPrice;
                textMeshPro.text = money.ToString();
                PlayerDataManager.Instance.AddCoin(itemPrice);
                ShopManager.Instance.ShowCoin();
        
                hasBought = true;
                purchaseCount++;
                
                yield return new WaitForSeconds(purchaseTime);
            }
        
            // 离开商店
            if (OnLeave != null) OnLeave();
            yield return new WaitForSeconds(leaveTime);
            Destroy(gameObject, 1f); // 销毁客人对象
        }
        
        private ShelfSlot FindBestAffordableItem()
        {
            ShelfSlot bestSlot = null;
            int highestPrice = 0;
            
            foreach (ShelfSlot slot in ShelfManager.Instance.shelfSlots)
            {
                if (slot.GetCurrentItem() != null)
                {
                    int price = slot.GetPrice();
                    if (price <= money && price > highestPrice)
                    {
                        highestPrice = price;
                        bestSlot = slot;
                    }
                }
            }
            
            return bestSlot;
        }
    }