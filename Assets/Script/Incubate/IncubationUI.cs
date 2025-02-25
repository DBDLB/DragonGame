using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncubationUI : MonoBehaviour
{
    public static IncubationUI instance;
    public static IncubationUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<IncubationUI>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(IncubationUI).Name);
                    instance = singleton.AddComponent<IncubationUI>();
                }
            }
            return instance;
        }
    }
    
    // 孵化 UI 元素
    public TextMeshProUGUI eggNameText;
    public TextMeshProUGUI incubationTimeText;
    public Button startButton;
    public Image hatchingEggImage;

    // 孵蛋控制器
    public EggIncubatorController incubatorController;

    private DragonEgg selectedEgg;

    private void Start()
    {
        // 默认状态不显示孵蛋信息
        eggNameText.gameObject.SetActive(false);
        incubationTimeText.gameObject.SetActive(false);
        // startButton.gameObject.SetActive(false);

        // 注册孵蛋按钮的点击事件
        startButton.onClick.AddListener(OnStartIncubation);
    }

    // 显示孵蛋信息
    public void ShowEggDetails(DragonEgg egg)
    {
        selectedEgg = egg;
        eggNameText.text = "选择的龙蛋: " + egg.itemName;
        incubationTimeText.text = "孵化时间: " + egg.incubationTime + "秒";

        eggNameText.gameObject.SetActive(true);
        incubationTimeText.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
    }

    // 开始孵化
    private void OnStartIncubation()
    {
        if (selectedEgg != null)
        {
            hatchingEggImage.sprite = selectedEgg.icon;
            hatchingEggImage.color = Color.white;
            selectedEgg.StartIncubation();
            incubatorController.StartIncubation(selectedEgg);
            incubatorController.GetComponent<Button>().interactable = false;
        }
    }
    
    //结束孵化
    public void OnEndIncubation()
    {
        hatchingEggImage.sprite = null;
        hatchingEggImage.color = new Color(0, 0, 0, 0);
        incubatorController.GetComponent<Image>().sprite = null;
        IncubationUI.Instance.incubatorController.GetComponent<Button>().interactable = true;
    }

    // 更新孵化进度显示（可以根据需要做进度条等）
    public void UpdateIncubationProgress()
    {
        if (selectedEgg != null && selectedEgg.status == EggStatus.InProgress)
        {
            float remainingTime = selectedEgg.incubationTime - (Time.time - selectedEgg.incubationStartTime);
            incubationTimeText.text = "剩余时间: " + Mathf.Max(remainingTime, 0) + "秒";
        }
    }
}