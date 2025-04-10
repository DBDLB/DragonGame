using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDispatchLocation : MonoBehaviour
{
    public GameObject DispatchLocationButton;
    public GameObject Content;
    
    public List<GameObject> slots = new List<GameObject>();
    private void OnEnable()
    {
        //删除Content下的所有子物体
        foreach (var slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
        
        foreach (var location in DispatchManager.Instance.dispatchLocation.allLocations)
        {
            //生成一个DragonEggButton到Content下
            GameObject slot = Instantiate(DispatchLocationButton, Content.transform);
            slot.GetComponent<DispatchLocationButton>().locationID = location.id;
            slot.GetComponentInChildren<Image>().sprite = DispatchManager.Instance.GetSpriteByID(location.id);
            slots.Add(slot);
        }
    }
    
}
