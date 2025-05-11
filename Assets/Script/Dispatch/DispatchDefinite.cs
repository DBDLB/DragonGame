using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class DispatchDefinite : MonoBehaviour
{
    public static DispatchDefinite instance;
    public static DispatchDefinite Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DispatchDefinite>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(DispatchDefinite).Name);
                    instance = singleton.AddComponent<DispatchDefinite>();
                }
            }
            return instance;
        }
    }
    [HideInInspector]
    public TextMeshProUGUI textMeshProUGUI;
    private Button button;
    private bool isButtonPressed = false;
    private float responseInterval = 0.5f;
    
    private void OnEnable()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponentInChildren<Button>();
        
        // 添加事件触发器
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? 
                              button.gameObject.AddComponent<EventTrigger>();
        
        // 添加按下事件
        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { OnButtonPressed(); });
        trigger.triggers.Add(pointerDown);
        
        // 添加抬起事件
        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
        trigger.triggers.Add(pointerUp);
        
        // 添加移出事件
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => { OnButtonReleased(); });
        trigger.triggers.Add(pointerExit);
    }

    [HideInInspector]
    public int clickCount = 0;
    private void OnButtonPressed()
    {

        if (clickCount<=2&&DispatchManager.Instance.selectedDragon != null&&DispatchManager.Instance.selectedDragon.id != 0&&DispatchManager.Instance.locationID!=0)
        {
            Debug.Log(clickCount);
            DispatchManager.Instance.newTask.remainingTime -= 1;
            clickCount++;
        }
        isButtonPressed = true;

        StartCoroutine(ButtonHoldCoroutine());
    }
    
    private void OnButtonReleased()
    {
        isButtonPressed = false;
        StopAllCoroutines();
    }
    
    private IEnumerator ButtonHoldCoroutine()
    {
        while (isButtonPressed)
        {
            yield return new WaitForSeconds(responseInterval);
            if (isButtonPressed)
            {
                DispatchManager.Instance.newTask.remainingTime -= 1;
            }
        }
    }
    
    public void ProcessButtonAction()
    {
        if (DispatchManager.Instance.newTask.isCompleted)
        {
            DispatchManager.Instance.StartDispatch();
        }
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}