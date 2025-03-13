using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "allDispatchLocation", menuName = "Dispatch/allDispatchLocation")]
public class DispatchLocation : ScriptableObject
{
    [System.Serializable]
    public class Location
    {
        public int id;
        public string locationName;
        public float baseValue; // 基础值
        public float baseTime;  // 基础时间
    }
    
    public List<Location> allLocations = new List<Location>();
}
