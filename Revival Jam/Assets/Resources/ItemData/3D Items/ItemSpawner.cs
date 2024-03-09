using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item3D genericItem3D;
    [field: SerializeField] public ItemData3D ToolData { get; private set; }

    public static event Action<ItemSpawner> GetSpawnReference;

    Item3D item3D;
    
    public void Start()
    {
        item3D = Instantiate(genericItem3D, transform);
        item3D.gameObject.SetActive(false);
        GetSpawnReference?.Invoke(this);
    }

    public void SpawnItem()
    {
        item3D.gameObject.SetActive(true);
        item3D.InitializeData(ToolData);
    }
}
