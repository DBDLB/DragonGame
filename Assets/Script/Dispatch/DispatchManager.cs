using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DispatchManager : MonoBehaviour
{
    
    public static DispatchManager instance;
    public static DispatchManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DispatchManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(DispatchManager).Name);
                    instance = singleton.AddComponent<DispatchManager>();
                }
            }
            return instance;
        }
    }
    public class DispatchTask
    {
        public int locationID;// 派遣地点ID
        public Dragon assignedDragon;// 派遣的龙
        public float remainingTime;// 剩余时间
        public bool isCompleted=true;// 是否完成
    }
    public DispatchLocation dispatchLocation;  // 所有派遣地点
    private List<DispatchTask> activeTasks = new List<DispatchTask>();
    [HideInInspector]
    public int locationID;
    [HideInInspector]
    public Dragon selectedDragon;

    public bool getSpoilsOfWar = false;
    // public Slider dispatchSlider;
    public Image dispatchSlider;
    
    
    //暂时
    public Image dragonImage;
    public Image locationImage;
    public GameObject showSpoilsOfWar;
    public Sprite defaultSprite;

    [HideInInspector] public DispatchTask newTask = new DispatchTask();

    public void Start()
    {
        dispatchSlider.material.SetFloat("_Progress", 1);
        dispatchSlider.material.SetColor("_Color", new Color(1, 1, 1, 1));
    }

    private void Update()
    {
        if (newTask.isCompleted)
        {
            if (selectedDragon != null&&selectedDragon.id != 0&&locationID!=0)
            {
                DispatchLocation.Location location = dispatchLocation.allLocations.Find(l => l.id == DispatchManager.Instance.locationID);
                float DispatchTime = CalculateDispatchTime(location, selectedDragon);
                dispatchSlider.GetComponentInChildren<TextMeshProUGUI>().text = $"任务预计时间：{(int)DispatchTime} 秒";
            }
        }
    }
    public void StartDispatch()
    {
        if (selectedDragon == null||selectedDragon.id == 0||locationID==0)
        {
            Debug.Log("请选择龙和地点！");
            return;
        }
        DispatchDefinite.Instance.textMeshProUGUI.text = "加油打气！";
        DispatchLocation.Location location = dispatchLocation.allLocations.Find(l => l.id == locationID);
        if (location == null) return;

        float DispatchTime = CalculateDispatchTime(location, selectedDragon);

        newTask = new DispatchTask()
        {
            locationID = location.id,
            assignedDragon = selectedDragon,
            remainingTime = DispatchTime,
            isCompleted = false
        };

        activeTasks.Add(newTask);
        StartCoroutine(DispatchCountdown(newTask));
        // Debug.Log($"派遣龙 {selectedDragon.itemName} 到 {location.locationName}，预计时间：{(int)DispatchTime} 秒");
    }

    private IEnumerator DispatchCountdown(DispatchTask task)
    {
        // dispatchSlider.maxValue = task.remainingTime;
        float maxTime = task.remainingTime;
        while (task.remainingTime > 0)
        {
            yield return new WaitForSeconds(0.02f);
            task.remainingTime -= 0.02f;
            task.remainingTime = task.remainingTime;
            DispatchDefinite.Instance.clickCount = 0;
            dispatchSlider.material.SetFloat("_Progress", 1-task.remainingTime / maxTime);
            // Debug.Log($"任务剩余时间：{(int)task.remainingTime} 秒");
            dispatchSlider.GetComponentInChildren<TextMeshProUGUI>().text = $"任务剩余时间：{(int)task.remainingTime} 秒";
        }
        dispatchSlider.GetComponentInChildren<TextMeshProUGUI>().text = "任务完成！";
        task.isCompleted = true;
        OnDispatchComplete(task);
    }

    private void OnDispatchComplete(DispatchTask task)
    {
        Debug.Log($"派遣完成！{task.assignedDragon.itemName} 从 {task.locationID} 归来，获得战利品！");
        selectedDragon = null;
        dragonImage.sprite = null;
        locationID = 0;
        dispatchSlider.sprite = defaultSprite;
        DispatchManager.Instance.dispatchSlider.material.SetColor("_Color", new Color(3, 3, 3, 1));
        DispatchDefinite.Instance.textMeshProUGUI.text = "派遣完成！";
        getSpoilsOfWar = true;
        // 处理战利品奖励逻辑
        activeTasks.Remove(task);
    }
    
    // 计算派遣时间
    public float CalculateDispatchTime(DispatchLocation.Location location, Dragon dragon)
    {
        float sumStats = dragon.attack + dragon.health + dragon.defense;
        float speedMultiplier = Mathf.Max((float)dragon.speed / 100,1);
        // 派遣时间 = （基础值 / （龙的总属性 * 速度乘数）） * 基础时间
        float DispatchTime = (location.baseValue / (sumStats * speedMultiplier)) * location.baseTime;
        return Mathf.Max(DispatchTime, 1); // 确保最小时间为1秒
    }
    
    //获取战利品
    public void GetSpoilsOfWar()
    {
        showSpoilsOfWar.GetComponent<ShowSpoilsOfWar>().items = SelectRandomID();
    }
    
    private int minID = 1;                 // ID 范围最小值
    private int maxID = 4;               // ID 范围最大值
    // 随机选择一个 ID
    List<Item> SelectRandomID()
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < Random.Range(2, 10); i++)
        {
            int selectedID = Random.Range(minID, maxID);
            Debug.Log("Selected ID: " + selectedID);
            items.Add(ItemManager.Instance.InstantiateItem(selectedID,ItemType.DragonEgg));
        }
        return items;
    }
}
