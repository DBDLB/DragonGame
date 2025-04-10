using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EggIncubatorController : MonoBehaviour
{
    // 当前孵化的龙蛋
    public DragonEgg currentEgg;
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "incubation_data.json");
    }
    
    private float autoSaveInterval = 60f; // 每60秒自动保存一次
    private float timeSinceLastSave = 0f;
    private void Update()
    {
        // 更新孵化进度
        if (currentEgg != null && currentEgg.status == EggStatus.InProgress)
        {
            // 显示进度
            IncubationUI.Instance.UpdateIncubationProgress(currentEgg);

            // 如果孵化完成，则生成龙
            if (currentEgg.IsIncubationComplete())
            {
                IncubationUI.Instance.egg = currentEgg;
                IncubationUI.Instance.hatchingEggImage.GetComponent<Button>().interactable = true;
            }
        }
        
        // 自动保存逻辑
        timeSinceLastSave += Time.deltaTime;
        if (timeSinceLastSave >= autoSaveInterval)
        {
            SaveIncubationProgress();
            timeSinceLastSave = 0f;
            Debug.Log("自动保存任务数据完成");
        }
    }
    
    // 在游戏退出时保存进度
    private void OnApplicationQuit()
    {
        SaveIncubationProgress();
    }

    // 开始孵化
    public void StartIncubation(DragonEgg egg)
    {
        currentEgg = egg;
        SaveIncubationProgress();
    }

    // 保存孵化进度
    public void SaveIncubationProgress()
    {
        if (currentEgg != null)
        {
            IncubationSaveData saveData = new IncubationSaveData
            {
                eggData = new InventoryManager.InventoryDragonEggData
                {
                itemID = currentEgg.itemID, 
                quantity = currentEgg.quantity,
                dragonEggs = new DragonEggData
                (
                    currentEgg.id,
                    currentEgg.itemName,
                    currentEgg.icon.name,
                    currentEgg.description,
                        
                    // description = dragonEgg.description,
                    // isStackable = item.isStackable.ToString(),
                    currentEgg.eggModelAdress,
                    currentEgg.eggBornTime,
                    currentEgg.eggPrice,
                    currentEgg.bornDragonId,
                    currentEgg.bornDragonPro
                )
                },
                remainingTime = currentEgg.eggBornTime - (Time.time - currentEgg.incubationStartTime),
                status = currentEgg.status
            };

            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(savePath, json);
            Debug.Log("孵化进度已保存");
        }
        else if (File.Exists(savePath))
        {
            // 如果没有正在孵化的蛋，删除保存文件
            File.Delete(savePath);
        }
    }

    // 加载孵化进度
    public void LoadIncubationProgress()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            IncubationSaveData saveData = JsonUtility.FromJson<IncubationSaveData>(json);
            
            if (saveData != null && saveData.status == EggStatus.InProgress)
            {
                Sprite icon = Resources.Load<Sprite>("Icons/" + saveData.eggData.dragonEggs.icon);
                // List<Vector2> hatchedDragons = new List<Vector2> { saveData.eggData.dragonEggs.bornDragonA, saveData.eggData.dragonEggs.bornDragonB };
                DragonEgg egg = new DragonEgg(
                    saveData.eggData.dragonEggs.itemName, 
                    ItemType.DragonEgg, 
                    1, 
                    icon, 
                    // bool.Parse(eggData.isStackable), 
                    saveData.eggData.dragonEggs.eggBornTime, 
                    saveData.eggData.dragonEggs.bornDragonId,
                    saveData.eggData.dragonEggs.bornDragonPro,
                    saveData.eggData.dragonEggs.id, 
                    saveData.eggData.itemID,
                    saveData.eggData.dragonEggs.description,
                    saveData.eggData.dragonEggs.eggModelAdress,
                    saveData.eggData.dragonEggs.eggPrice
                );
                egg.quantity = saveData.eggData.quantity;
                
                if (egg != null)
                {
                    currentEgg = egg;
                    // 根据剩余时间重新计算开始时间
                    currentEgg.incubationStartTime = Time.time - (egg.eggBornTime - saveData.remainingTime);
                    currentEgg.status = saveData.status;
                    
                    // 更新UI
                    IncubationUI.Instance.hatchingEggImage.gameObject.SetActive(true);
                    IncubationUI.Instance.hatchingEggImage.sprite = currentEgg.icon;
                    IncubationUI.Instance.hatchingEggImage.color = Color.white;
                    IncubationUI.Instance.IncubateButton.interactable = false;
                    IncubationUI.Instance.IncubateProgressBar.gameObject.SetActive(true);
                    
                    Debug.Log("孵化进度已恢复");
                }
            }
        }
    }

    // 在应用暂停时保存进度（针对移动平台）
    // private void OnApplicationPause(bool pauseStatus)
    // {
    //     if (pauseStatus)
    //     {
    //         SaveIncubationProgress();
    //     }
    // }
    
    [System.Serializable]
    public class IncubationSaveData
    {
        public InventoryManager.InventoryDragonEggData eggData;        // 孵化中的龙蛋itemID
        public float remainingTime;  // 剩余孵化时间
        public EggStatus status;     // 孵化状态
    }
}
