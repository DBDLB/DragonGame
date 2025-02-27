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
    public float incubationTime;

    // 当前孵化状态
    public EggStatus status;

    // 孵化开始时间
    public float incubationStartTime;

    // 孵化完成后的龙（孵化完成后生成的龙）
    public Dragon hatchedDragon;

    private void Start()
    {
        // 初始状态为未孵化
        status = EggStatus.NotStarted;
        // itemName = this.gameObject.name;
        Inventory.Instance.AddItem(this);
    }

    // 启动孵化
    public void StartIncubation()
    {
        status = EggStatus.InProgress;
        incubationStartTime = Time.time;
    }

    // 检查孵化是否完成
    public bool IsIncubationComplete()
    {
        return Time.time - incubationStartTime >= incubationTime;
    }

    // 完成孵化，生成龙
    public void HatchEgg()
    {
        if (IsIncubationComplete() && status == EggStatus.InProgress)
        {
            status = EggStatus.Hatched;
            hatchedDragon = new Dragon(itemName);  // 生成一个龙
        }
    }
}