using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;

public class Interface : MonoBehaviour
{
    [SerializeField] InterfaceData InterfaceData;

    public static event Action<ItemData> UseItem;


    private void Update()
    {
        IsMouseOver();

        if (Input.GetMouseButtonUp(0) && IsMouseOver())
            UseItemOnInterface();
    }

    bool IsMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 5, InterfaceData.InterfaceLayer))
        {
            Debug.Log(hit.transform.name);

            return true;
        }
        return false;

    }


    void UseItemOnInterface()
    {
        var holdingItem = HotbarManager.Instance.HoldingItem;
        if (holdingItem == null || holdingItem.ItemType != InterfaceData.InterfaceTool)
        {
            if (holdingItem != null)
                Debug.Log($"holding wrong item | holding : {holdingItem.ItemType} - Required Item : {InterfaceData.InterfaceTool}");
            else
                Debug.Log($"Not holding an item | holdingItem is null {holdingItem == null}");

            return;
        }

        if (PlayerFocus.Instance.Focused != InterfaceData.InterfaceType)
            return;

        Debug.Log($"Used Item on Interface ({InterfaceData.name})");

        UseItem?.Invoke(holdingItem);
    }
}
