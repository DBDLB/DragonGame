using System.Collections.Generic;
using System.Linq;
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
    
    //龙蛋直接出售价格
    public float sellPrice;
    
    // 龙蛋上架出售价格
    public float listPrice;

    // 当前孵化状态
    public EggStatus status;

    // 孵化开始时间
    public float incubationStartTime;

    // 孵化完成后的龙（孵化完成后生成的龙）
    public string bornDragonId;
    public string bornDragonPro;

    public DragonEgg(string name, ItemType type, int quantity, Sprite icon,float eggBornTime,string bornDragonId,string bornDragonPro,int id,int itemID,string description,string eggModelAdress,float sellPrice,float listPrice) : base(name, type, quantity, icon,id,itemID,description)
    {
        // 初始状态为未孵化
        status = EggStatus.NotStarted;
        // itemName = this.gameObject.name;
        // Inventory.Instance.AddItem(this);
        this.eggBornTime = eggBornTime;
        this.bornDragonId = bornDragonId;
        this.bornDragonPro = bornDragonPro;
        this.eggModelAdress = eggModelAdress;
        this.sellPrice = sellPrice;
        this.listPrice = listPrice;
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
    // 将字符串转换为数组
    string[] bornDragonIdArray = StringToArray(this.bornDragonId);
    string[] bornDragonProArray = StringToArray(this.bornDragonPro);
    // 检查参数是否有效
    if (bornDragonIdArray == null || bornDragonProArray == null || bornDragonIdArray.Length == 0 || bornDragonProArray.Length == 0 || bornDragonIdArray.Length != bornDragonProArray.Length)
    {
        Debug.LogError("龙蛋数据有误: bornDragonId或bornDragonPro无效");
        return -1;
    }

    // 计算所有权重的总和
    float totalWeight = 0;
    for (int i = 0; i < bornDragonProArray.Length; i++)
    {
        if (float.TryParse(bornDragonProArray[i], out float weight))
        {
            totalWeight += weight;
        }
        else
        {
            Debug.LogError($"权重转换错误: {bornDragonProArray[i]}不是有效的数字");
        }
    }

    // 生成0到totalWeight之间的随机数
    float randomValue = Random.Range(0, totalWeight);

    // 遍历数组，累加权重直到达到或超过随机值
    float currentWeight = 0;
    for (int i = 0; i < bornDragonIdArray.Length; i++)
    {
        if (float.TryParse(bornDragonProArray[i], out float weight))
        {
            currentWeight += weight;
            
            // 当累计权重超过随机值时，返回当前龙ID
            if (currentWeight >= randomValue)
            {
                if (int.TryParse(bornDragonIdArray[i], out int dragonId))
                {
                    return dragonId;
                }
            }
        }
    }

    // 保险起见，返回第一个有效的龙ID
    if (int.TryParse(bornDragonIdArray[0], out int firstDragonId))
    {
        return firstDragonId;
    }
    
    return -1;
}

// 辅助方法：将字符串转换为字符串数组
private string[] StringToArray(string input)
{
    if (string.IsNullOrEmpty(input))
        return new string[0];

    // 按逗号分割字符串并去除空格
    string[] values = input.Split(',')
        .Select(s => s.Trim())
        .ToArray();

    return values;
}
    
// 辅助方法：将字符串数组转换为字符串
private string ArrayToString(string[] array)
{
    if (array == null || array.Length == 0)
        return string.Empty;

    // 使用逗号连接字符串数组
    return string.Join(",", array);
}


}