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
    public int hatchedDragonId;

    public DragonEgg(string name, ItemType type, int quantity, Sprite icon, bool stackable,float incubationTime,int hatchedDragonId) : base(name, type, quantity, icon, stackable)
    {
        // 初始状态为未孵化
        status = EggStatus.NotStarted;
        // itemName = this.gameObject.name;
        // Inventory.Instance.AddItem(this);
        this.incubationTime = incubationTime;
        this.hatchedDragonId = hatchedDragonId;
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
        return Time.time - incubationStartTime >= incubationTime;
    }

    // 完成孵化，生成龙
    public Dragon HatchEgg()
    {
        Dragon dragon;
        if (IsIncubationComplete() && status == EggStatus.InProgress)
        {
            status = EggStatus.Hatched;
            if (hatchedDragonId>0)
            {
                // 生成龙
                dragon = (Dragon)ItemManager.Instance.InstantiateItem(hatchedDragonId);
            }
            else
            {
                dragon = null;
            }
            return dragon;
        }
        return null;
    }


}