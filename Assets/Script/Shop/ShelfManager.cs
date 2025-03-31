using System.Collections.Generic;
    using UnityEngine;
    
    public class ShelfManager : MonoBehaviour
    {
        public static ShelfManager Instance;
        public List<ShelfSlot> shelfSlots = new List<ShelfSlot>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            // 获取所有货架槽位
            shelfSlots.AddRange(GetComponentsInChildren<ShelfSlot>());
        }
        
        // 添加更多货架管理功能，如收入计算、自动销售等
    }