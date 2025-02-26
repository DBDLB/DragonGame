using System;
using UnityEngine;
using UnityEngine.UI;

public class EggIncubatorController : MonoBehaviour
{
    // 当前孵化的龙蛋
    private DragonEgg currentEgg;

    private void OnEnable()
    {
        currentEgg = null;
    }

    private void Update()
    {
        // 更新孵化进度
        if (currentEgg != null && currentEgg.status == EggStatus.InProgress)
        {
            // 显示进度
            IncubationUI.Instance.UpdateIncubationProgress();

            // 如果孵化完成，则生成龙
            if (currentEgg.IsIncubationComplete())
            {
                IncubationUI.Instance.egg = currentEgg;
                IncubationUI.Instance.hatchingEggImage.GetComponent<Button>().interactable = true;
            }
        }
    }

    // 开始孵化
    public void StartIncubation(DragonEgg egg)
    {
        currentEgg = egg;
    }
}