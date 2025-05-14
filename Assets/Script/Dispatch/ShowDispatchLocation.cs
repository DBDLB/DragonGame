using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDispatchLocation : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Image locationImage;
    public const int DEFAULT_LOCATION_ID = 1051;

    private int currentLocationIndex = 0;
    private List<DispatchManager.DispatchLocation.Location> availableLocations;

    private void OnEnable()
    {
        InitializeAvailableLocations();
        RegisterButtonEvents();
        ShowCurrentLocation();
    }

    private void InitializeAvailableLocations()
    {
        availableLocations = DispatchManager.Instance.dispatchLocation.allLocations;
        
        // 确保从默认地点开始
        currentLocationIndex = availableLocations.FindIndex(loc => loc.id == DEFAULT_LOCATION_ID);
        if (currentLocationIndex == -1)
        {
            currentLocationIndex = 0;
            Debug.LogWarning($"未找到ID为{DEFAULT_LOCATION_ID}的默认地点");
        }
    }

    private void RegisterButtonEvents()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void OnDisable()
    {
        leftButton.onClick.RemoveListener(OnLeftButtonClick);
        rightButton.onClick.RemoveListener(OnRightButtonClick);
    }

    private void OnLeftButtonClick()
    {
        if (availableLocations.Count <= 1) return;

        currentLocationIndex--;
        if (currentLocationIndex < 0)
        {
            currentLocationIndex = availableLocations.Count - 1;
        }
        ShowCurrentLocation();
    }

    private void OnRightButtonClick()
    {
        if (availableLocations.Count <= 1) return;

        currentLocationIndex++;
        if (currentLocationIndex >= availableLocations.Count)
        {
            currentLocationIndex = 0;
        }
        ShowCurrentLocation();
    }

    private void ShowCurrentLocation()
    {
        if (availableLocations.Count > 0 && currentLocationIndex >= 0 && currentLocationIndex < availableLocations.Count)
        {
            var location = availableLocations[currentLocationIndex];
            locationImage.sprite = DispatchManager.Instance.GetSpriteByID(location.id);
            DispatchManager.Instance.locationID = location.id;
            DispatchManager.Instance.dispatchSlider.sprite = locationImage.sprite;
        }
    }
}
