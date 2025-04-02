using System.Collections;
    using UnityEngine;
    
    public class CustomerSpawner : MonoBehaviour
    {
        public static CustomerSpawner Instance;
        
        [SerializeField] private GameObject customerPrefab;
        [SerializeField] private Transform spawnPoint;
        
        // 商店等级和客人属性
        [SerializeField] private int shopLevel = 1;// 商店等级
        [SerializeField] private float baseSpawnInterval = 30f;// 客人生成间隔
        [SerializeField] private int baseMinMoney = 50;//客人携带金钱范围
        [SerializeField] private int baseMaxMoney = 500; //客人携带金钱范围
        
        // 龙词条提供的修饰符
        private float dragonIntervalModifier = 0f; // 生成间隔修饰符
        private float dragonMoneyModifier = 0f; // 金钱修饰符
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            StartCoroutine(SpawnCustomersRoutine());
        }
        
        private IEnumerator SpawnCustomersRoutine()
        {
            while (true)
            {
                // 只有当有商品可售时才生成客人
                if (AreItemsAvailableForSale())
                {
                    // 根据商店等级和龙词条计算实际生成间隔
                    float actualInterval = CalculateSpawnInterval();
                    yield return new WaitForSeconds(actualInterval);
            
                    // 再次检查，确保在等待期间商品没有被全部卖出
                    if (AreItemsAvailableForSale())
                    {
                        SpawnCustomer();
                    }
                }
                else
                {
                    // 如果没有商品，短暂等待后再次检查
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        
        private bool AreItemsAvailableForSale()
        {
            // 如果ShelfManager不存在，返回false
            if (ShelfManager.Instance == null || ShelfManager.Instance.shelfSlots == null)
                return false;
        
            // 检查是否有至少一个货架槽位中有商品
            foreach (ShelfSlot slot in ShelfManager.Instance.shelfSlots)
            {
                if (slot != null && slot.GetCurrentItem() != null)
                    return true;
            }
    
            return false;
        }
        
        private float CalculateSpawnInterval()
        {
            // 商店等级越高，生成间隔越短
            float levelModifier = Mathf.Max(0.5f, 1f - (shopLevel * 0.05f)); // 每级减少5%，最低减少50%
            return Mathf.Max(5f, baseSpawnInterval * levelModifier - dragonIntervalModifier);
        }
        
        private void SpawnCustomer()
        {
            GameObject customerObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.transform);
            Customer customer = customerObj.GetComponent<Customer>();
            
            // 计算客人携带的金钱范围
            int actualMinMoney = baseMinMoney + (shopLevel * 50);
            int actualMaxMoney = baseMaxMoney + (shopLevel * 100);
            
            // 应用龙词条修饰符
            actualMinMoney = Mathf.RoundToInt(actualMinMoney * (1 + dragonMoneyModifier));
            actualMaxMoney = Mathf.RoundToInt(actualMaxMoney * (1 + dragonMoneyModifier));
            
            // 分配随机金钱
            customer.money = Random.Range(actualMinMoney, actualMaxMoney + 1);
            Debug.Log($"客人携带金币: {customer.money}");
        }
        
        // 更新商店等级
        public void UpdateShopLevel(int level)
        {
            shopLevel = level;
        }
        
        // 更新龙词条修饰符
        public void UpdateDragonModifiers(float intervalMod, float moneyMod)
        {
            dragonIntervalModifier = intervalMod;
            dragonMoneyModifier = moneyMod;
        }
    }