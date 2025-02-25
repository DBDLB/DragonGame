public class Dragon
{
    public string dragonName;

    // 构造函数
    public Dragon(string eggName)
    {
        // 生成龙的名字或属性，可以根据蛋的名字生成不同的龙
        this.dragonName = "龙_" + eggName;
    }
}