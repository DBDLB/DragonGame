using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json;

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
        public float totalTime;// 总时间
        public bool isCompleted=true;// 是否完成
        
        public DispatchTask(int locationID, Dragon assignedDragon, float remainingTime, float totalTime, bool isCompleted)
        {
            this.locationID = locationID;
            this.assignedDragon = assignedDragon;
            this.remainingTime = remainingTime;
            this.totalTime = totalTime;
            this.isCompleted = isCompleted;
        }
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
    public GameObject showSpoilsOfWar;
    public Sprite defaultSprite;

    [HideInInspector] public DispatchTask newTask = new DispatchTask(0, null, 0, 0, true);

    public void Start()
    {
        LoadTasks();
    }
    
    private void Awake()
    {
        dispatchLocation = new DispatchLocation();
        LoadDispatchLocations();
    }
    
    private void OnApplicationQuit()
    {
        // 游戏退出时保存任务数据
        SaveTasks();
        Debug.Log("游戏退出，任务数据已保存");
    }

    private float autoSaveInterval = 60f; // 每60秒自动保存一次
    private float timeSinceLastSave = 0f;
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
        
        // 自动保存逻辑
        timeSinceLastSave += Time.deltaTime;
        if (timeSinceLastSave >= autoSaveInterval)
        {
            SaveTasks();
            timeSinceLastSave = 0f;
            Debug.Log("自动保存任务数据完成");
        }
    }

    #region 派遣地点数据加载

    // 初始化时加载派遣地点数据
    private void LoadDispatchLocations()
    {
        string filePath = "Data/DispatchLocationData";  // 不需要.json后缀
        TextAsset jsonText = Resources.Load<TextAsset>(filePath);
        
        if (jsonText != null)
        {
            // 使用JsonUtility
            // DispatchLocation.Location[] locations = JsonUtility.FromJson<DispatchLocation.Location[]>("{\"locations\":" + jsonText.text + "}").locations;
            
            // 或使用Newtonsoft.Json
            DispatchLocation.Location[] locations = JsonConvert.DeserializeObject<DispatchLocation.Location[]>(jsonText.text);
            
            if (locations != null)
            {
                dispatchLocation.allLocations.AddRange(locations);
                Debug.Log($"成功加载了{locations.Length}个派遣地点");
            }
        }
        else
        {
            dispatchSlider.material.SetFloat("_Progress", 1);
            dispatchSlider.material.SetColor("_Color", new Color(1, 1, 1, 1));
            Debug.LogError("未找到派遣地点数据文件: " + filePath);
        }
    }
    public Sprite GetSpriteByID(int id)
    {
        foreach (var location in dispatchLocation.allLocations)
        {
            if (location.id == id)
            {
                // 这里假设你有一个方法可以根据ID获取对应的Sprite
                return Resources.Load<Sprite>("Icons/" + location.icon);
            }
        }
        return null;
    }

    #endregion

    
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

        newTask = new DispatchTask(location.id, selectedDragon, DispatchTime, DispatchTime, false);

        activeTasks.Add(newTask);
        StartCoroutine(DispatchCountdown(newTask));
        // Debug.Log($"派遣龙 {selectedDragon.itemName} 到 {location.locationName}，预计时间：{(int)DispatchTime} 秒");
    }

    private IEnumerator DispatchCountdown(DispatchTask task)
    {
        // dispatchSlider.maxValue = task.remainingTime;
        dispatchSlider.sprite = GetSpriteByID(task.locationID);
        float maxTime = task.totalTime;
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
        dispatchSlider.material.SetColor("_Color", new Color(3, 3, 3, 1));
        dispatchSlider.GetComponent<Button>().interactable = false;
        DispatchDefinite.Instance.textMeshProUGUI.text = "派遣完成！";
        getSpoilsOfWar = true;
        // 处理战利品奖励逻辑
        activeTasks.Remove(task);
    }
    
    // 计算派遣时间
    public float CalculateDispatchTime(DispatchLocation.Location location, Dragon dragon)
    {
        float sumStats = dragon.attack + dragon.life + dragon.defense;
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

    #region 任务数据保存与加载

    public void SaveTasks()
    {
        List<DispatchTaskData> taskDataList = new List<DispatchTaskData>();
        foreach (var task in activeTasks)
        {
            DispatchTaskData taskData = new DispatchTaskData
            {
                locationID = task.locationID,
                itemID = task.assignedDragon.itemID,
                remainingTime = task.remainingTime,
                totalTime = task.totalTime,
                isCompleted = task.isCompleted
            };
            taskDataList.Add(taskData);
        }

        TaskListWrapper wrapper = new TaskListWrapper { tasks = taskDataList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Application.persistentDataPath + "/tasks.json", json);
        // System.IO.File.WriteAllText(Application.persistentDataPath + "/tasks.json", json);
        Debug.Log("任务数据已保存！");
    }
    
    public void LoadTasks()
    {
        string path = Application.persistentDataPath + "/tasks.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            var taskDataList = JsonUtility.FromJson<TaskListWrapper>(json).tasks;

            foreach (var taskData in taskDataList)
            {
                Dragon assignedDragon = Inventory.Instance.GetByItemID(taskData.itemID) as Dragon;
                if (assignedDragon != null)
                {
                    DispatchTask task = new DispatchTask(taskData.locationID, assignedDragon, taskData.remainingTime, taskData.totalTime, taskData.isCompleted);
                    activeTasks.Add(task);

                    if (!task.isCompleted)
                    {
                        newTask = task;
                        DispatchDefinite.Instance.textMeshProUGUI.text = "加油打气！";
                        StartCoroutine(DispatchCountdown(task));
                    }
                }
            }
            Debug.Log("任务数据已加载！");
        }
        else
        {
            Debug.Log("未找到任务数据文件！");
        }
    }

    #endregion
    

    [System.Serializable]
    private class TaskListWrapper
    {
        public List<DispatchTaskData> tasks;
    }
    
    [System.Serializable]
    public class DispatchTaskData
    {
        public int locationID;
        public int itemID;
        public float remainingTime;
        public float totalTime;
        public bool isCompleted;
    }
    
    [System.Serializable]
    public class DispatchLocation
    {
        public List<Location> allLocations = new List<Location>();
        
        [System.Serializable]
        public class Location
        {
            public int id;
            public string name;
            public string icon;
            public string description;
            public int adventureValue;
            public float adventureTime;
            public float maxTime;
            public float minTime;
            public int reward;
            public float rewardPro;
            public int openConditions;
            
            // 计算派遣时间相关属性
            public float baseValue { get { return adventureValue; } }
            public float baseTime { get { return adventureTime; } }
            public string locationName { get { return description; } }
        }
    }
}
