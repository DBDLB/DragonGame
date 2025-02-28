using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GetThePrize : MonoBehaviour
{
    public Slider progressBar;            // 进度条
    public int minID = 1;                 // ID 范围最小值
    public int maxID = 4;               // ID 范围最大值
    public float timeToComplete = 5f;     // 进度条完成的时间（秒）

    private bool isComplete = false;

    // 用于显示选中的 ID
    public TextMeshProUGUI resultText;

    void Start()
    {
        // 确保进度条初始值为 0
        progressBar.value = 0;
        StartCoroutine(FillProgressBar());
    }

    // 进度条填充的协程
    IEnumerator FillProgressBar()
    {
        while (true)
        {
            float timeElapsed = 0f;

            while (timeElapsed < timeToComplete)
            {
                timeElapsed += Time.deltaTime;
                progressBar.value = timeElapsed / timeToComplete; // 计算进度条值
                yield return null;
            }

            // 进度条完成，随机选择一个 ID
            isComplete = true;
            SelectRandomID();
        }
    }

    // 随机选择一个 ID
    void SelectRandomID()
    {
        // 确保范围有效
        if (maxID >= minID)
        {
            int selectedID = Random.Range(minID, maxID + 1);
            Debug.Log("Selected ID: " + selectedID);

            // 显示结果
            if (resultText != null)
            {
                resultText.text = "Selected ID: " + selectedID;
            }
            ItemManager.Instance.InstantiateItem(selectedID);

            progressBar.value = 0;
        }
        else
        {
            Debug.LogError("Invalid ID range");
        }
    }
}