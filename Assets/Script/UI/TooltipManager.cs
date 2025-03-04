using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    
    public GameObject tooltipPanel; // 提示框面板
    public Vector2 offset = new Vector2(10, -10); // 鼠标偏移量

    private void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false); // 初始隐藏
    }

    private void Update()
    {
        // 让提示框跟随鼠标
        if (tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition + (Vector3)offset;
        }
    }

    public void ShowTooltip(string content)
    {
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}