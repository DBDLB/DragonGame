using UnityEngine;
    
    public class CustomerAnimationController : MonoBehaviour
    {
        private Customer customer;
        private Animator animator;
    
        private void Awake()
        {
            customer = GetComponent<Customer>();
            animator = GetComponent<Animator>();
            
            // 订阅事件
            customer.OnEnterShop += HandleEnterShop;
            customer.OnBrowse += HandleBrowse;
            customer.OnLeave += HandleLeave;
        }
    
        private void OnDestroy()
        {
            // 取消订阅事件以防止内存泄漏
            if (customer != null)
            {
                customer.OnEnterShop -= HandleEnterShop;
                customer.OnBrowse -= HandleBrowse;
                customer.OnLeave -= HandleLeave;
            }
        }
    
        // 事件处理方法
        private void HandleEnterShop()
        {
            Debug.Log("客人进入商店");
            animator.SetTrigger("Enter");
            // 播放进入动画或声音
        }
    
        private void HandleBrowse()
        {
            Debug.Log("客人正在浏览");
            animator.SetTrigger("Browse");
            // 播放浏览动画
        }
    
        private void HandleLeave()
        {
            Debug.Log("客人离开商店");
            animator.SetTrigger("Leave");
            // 播放离开动画
        }
    }