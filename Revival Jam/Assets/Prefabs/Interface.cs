using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;

public class Interface : MonoBehaviour
{
    [SerializeField] InterfaceData InterfaceData;
    [field: SerializeField] public LayerMask InterfaceLayer { get; private set; }


    public static event Action<ItemData> UseItem;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && IsMouseOver())
            UseItemOnInterface();
    }

    //This can be changed to detect all interfaces instead, so we don't need so many layers
    bool IsMouseOver()
    {
        if (Camera.main == null)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 5, InterfaceLayer))
        {
            return hit.transform == transform;
        }
        return false;
    }


    void UseItemOnInterface()
    {
        var holdingItem = HotbarManager.Instance.HoldingItem;

        if (!InterfaceData.InterfaceType.Contains(PlayerFocus.Instance.Focused))
            return;

        if (holdingItem == null || holdingItem.ItemType != InterfaceData.InterfaceTool)
        {
            if (holdingItem != null)
                Debug.Log($"holding wrong item | holding : {holdingItem.ItemType} - Required Item : {InterfaceData.InterfaceTool}");
            else
                Debug.Log($"Not holding an item");

            return;
        }

        Debug.Log($"Used Item on Interface ({InterfaceData.name})");

        UseItem?.Invoke(holdingItem);
    }
}
