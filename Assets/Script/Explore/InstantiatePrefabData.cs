using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/InstantiatePrefabData")]
public class InstantiatePrefabData : ScriptableObject
{
    [System.Serializable]
    public class PrefabData
    {
        public ItemType itemType;  // 物品类型
        public GameObject itemPrefab;  // 引用 Prefab
    }
    public List<PrefabData> PrefabDatas = new List<PrefabData>();

}
