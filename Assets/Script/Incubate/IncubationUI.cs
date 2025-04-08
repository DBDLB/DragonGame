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
    public Image selectEgg;

    // 孵蛋控制器
    public EggIncubatorController incubatorController;
    public Button IncubateButton;
    public Slider IncubateProgressBar;

    private DragonEgg selectedEgg;

    private void Start()
    {
        // 默认状态不显示孵蛋信息
        eggNameText.gameObject.SetActive(false);
        incubationTimeText.gameObject.SetActive(false);
        IncubateProgressBar.gameObject.SetActive(false);
        hatchingEggImage.gameObject.SetActive(false);
        hatchingEggImage.GetComponent<Button>().interactable = false;
        startButton.interactable = false;
        // startButton.gameObject.SetActive(false);

        // 注册孵蛋按钮的点击事件
        startButton.onClick.AddListener(OnStartIncubation);
        
        incubatorController.LoadIncubationProgress();
    }

    // 显示孵蛋信息
    public void ShowEggDetails(DragonEgg egg)
    {
        selectedEgg = egg;
        eggNameText.text = "选择的龙蛋: " + egg.itemName;
        incubationTimeText.text = "孵化时间: " + egg.eggBornTime + "秒";

        eggNameText.gameObject.SetActive(true);
        incubationTimeText.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
    }

    // 开始孵化
    private void OnStartIncubation()
    {
        if (selectedEgg != null)
        {
            hatchingEggImage.gameObject.SetActive(true);
            hatchingEggImage.sprite = selectedEgg.icon;
            hatchingEggImage.color = Color.white;
            selectedEgg.StartIncubation();
            incubatorController.StartIncubation(selectedEgg);
            IncubateButton.interactable = false;
            IncubateProgressBar.gameObject.SetActive(true);
            Inventory.Instance.RemoveItem(selectedEgg);
        }
    }
    
    public void CancelIncubation()
    {
        if (selectedEgg != null)
        {
            selectEgg.sprite = null;
            IncubateButton.interactable = true;
            IncubateProgressBar.gameObject.SetActive(false);
            startButton.interactable = false;
            eggNameText.text = "选择的龙蛋: 无";
            incubationTimeText.text = "孵化时间: 无";
            hatchingEggImage.sprite = null;
            hatchingEggImage.color = new Color(0, 0, 0, 0);
            hatchingEggImage.gameObject.SetActive(false);
            hatchingEggImage.GetComponent<Button>().interactable = false;
            Inventory.Instance.AddItem(selectedEgg);
        }
    }
    
    //结束孵化
    public void OnEndIncubation()
    {
        selectEgg.sprite = null;
        IncubateButton.interactable = true;
        IncubateProgressBar.gameObject.SetActive(false);
        startButton.interactable = false;
        eggNameText.text = "选择的龙蛋: 无";
        incubationTimeText.text = "孵化时间: 无";
    }

    // 更新孵化进度显示（可以根据需要做进度条等）
    public void UpdateIncubationProgress(DragonEgg currentEgg)
    {
        // if (selectedEgg != null && selectedEgg.status == EggStatus.InProgress)
        {
            float remainingTime = currentEgg.eggBornTime - (Time.time - currentEgg.incubationStartTime);
            remainingTime = Mathf.Max(0, remainingTime);
            IncubateProgressBar.value = 1 - remainingTime / currentEgg.eggBornTime; ;
            IncubateProgressBar.GetComponentInChildren<TextMeshProUGUI>().text = remainingTime.ToString("F2") + "s";
        }
    }

    [HideInInspector] public DragonEgg egg;
    public GetDragonUI getDragonUI;
    //获取孵化结果
    public void GetHatchedDragon()
    {
        hatchingEggImage.sprite = null;
        hatchingEggImage.color = new Color(0, 0, 0, 0);
        hatchingEggImage.gameObject.SetActive(false);
        hatchingEggImage.GetComponent<Button>().interactable = false;
        Dragon dragon = egg.HatchEgg();
        Debug.Log("孵化完成，获得的龙是: " + dragon.itemName);
        getDragonUI.ShowDragonUI(dragon);
        OnEndIncubation();
    }
}