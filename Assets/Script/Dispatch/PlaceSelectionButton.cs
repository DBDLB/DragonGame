using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PlaceSelectionButton : MonoBehaviour
{
    public GameObject DispatchPrepare;
    private bool isButtonPressed = true;
    private void OnEnable()
    {
        // 注册按钮的点击事件
        GetComponentInChildren<Button>().onClick.AddListener(OnClicked);
        isButtonPressed = true;
    }
    private void OnClicked()
    {
        if (DispatchManager.Instance.getSpoilsOfWar)
        {
            DispatchManager.Instance.OnDispatchComplete(DispatchManager.Instance.newTask);
            DispatchManager.Instance.GetSpoilsOfWar();
            DispatchManager.Instance.getSpoilsOfWar = false;
            DispatchManager.Instance.showSpoilsOfWar.SetActive(true);
            // textMeshProUGUI.text = "确认出发";
            // DispatchManager.Instance.dragonImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "龙选择";
            DispatchManager.Instance.dispatchSlider.GetComponentInChildren<TextMeshProUGUI>().text = "";
            DispatchManager.Instance.dispatchSlider.material.SetFloat("_Progress", 1);
            DispatchManager.Instance.dispatchSlider.material.SetColor("_Color", new Color(1, 1, 1, 1));
            DispatchManager.Instance.dispatchSlider.sprite = DispatchManager.Instance.defaultSprite;
            DispatchManager.Instance.dispatchSlider.GetComponent<Button>().interactable = true;
            isButtonPressed = true;
        }
        else if(isButtonPressed)
        {
            DispatchPrepare.SetActive(true);
            isButtonPressed = false;
        }
    }
}
