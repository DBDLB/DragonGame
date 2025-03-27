using System.Collections.Generic;
using UnityEngine;

public enum EggStatus
{
    NotStarted,
    InProgress,
    Hatched
}

public class DragonEgg : Item
{
    // 孵化所需时间（秒）
    public float eggBornTime;
    
    //龙蛋模型地址
    public string eggModelAdress;
    
    //龙蛋价格
    public float eggPrice;

    // 当前孵化状态
    public EggStatus status;

    // 孵化开始时间
    public float incubationStartTime;

    // 孵化完成后的龙（孵化完成后生成的龙）
    public List<Vector2> hatchedDragons;

    public DragonEgg(string name, ItemType type, int quantity, Sprite icon,float eggBornTime,List<Vector2> hatchedDragons,int id,int itemID,string description,string eggModelAdress,float eggPrice) : base(name, type, quantity, icon,id,itemID,description)
    {
        // 初始状态为未孵化
        status = EggStatus.NotStarted;
        // itemName = this.gameObject.name;
        // Inventory.Instance.AddItem(this);
        this.eggBornTime = eggBornTime;
        this.hatchedDragons = hatchedDragons;
        this.eggModelAdress = eggModelAdress;
        this.eggPrice = eggPrice;
    }
    // private void Start()
    // {
    //
    // }

    // 启动孵化
    public void StartIncubation()
    {
        status = EggStatus.InProgress;
        incubationStartTime = Time.time;
    }

    // 检查孵化是否完成
    public bool IsIncubationComplete()
    {
        return Time.time - incubationStartTime >= eggBornTime;
    }

    // 完成孵化，生成龙
    public Dragon HatchEgg()
    {
        Dragon dragon = null;
        if (IsIncubationComplete() && status == EggStatus.InProgress)
        {
            status = EggStatus.Hatched;
        
            // 根据权重随机选择龙ID
            int selectedDragonId = GetRandomDragonIdByWeight();
        
            if (selectedDragonId > 0)
            {
                // 生成选中的龙
                dragon = (Dragon)ItemManager.Instance.InstantiateItem(selectedDragonId, ItemType.Dragon);
            }
        
            return dragon;
        }
        return null;
    }
    public int GetRandomDragonIdByWeight()
    {
        // 如果没有可能的龙，返回-1或抛出异常
        if (hatchedDragons == null || hatchedDragons.Count == 0)
            return -1;
    
        // 计算所有权重的总和
        float totalWeight = 0;
        foreach (Vector2 dragon in hatchedDragons)
        {
            totalWeight += dragon.y; // y值为权重
        }
    
        // 生成0到totalWeight之间的随机数
        float randomValue = Random.Range(0, totalWeight);
    
        // 遍历列表，累加权重直到达到或超过随机值
        float currentWeight = 0;
        foreach (Vector2 dragon in hatchedDragons)
        {
            currentWeight += dragon.y;
        
            // 当累计权重超过随机值时，返回当前龙ID
            if (currentWeight >= randomValue)
                return (int)dragon.x; // x值为龙ID
        }
    
        // 保险起见，返回列表中最后一个龙ID
        return (int)hatchedDragons[hatchedDragons.Count - 1].x;
    }


}