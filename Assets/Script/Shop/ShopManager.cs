using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ShopManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(ShopManager).Name);
                    instance = singleton.AddComponent<ShopManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }
    
    public TextMeshProUGUI coinText;

    private void Start()
    {
        ShowCoin();
    }

    public void ShowCoin()
    {
        coinText.text = "金币：" + PlayerDataManager.Instance.coin.ToString();
    }
    
    
}
