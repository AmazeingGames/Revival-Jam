using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interface : MonoBehaviour
{
    [SerializeField] InterfaceData InterfaceData;

    private void OnMouseUp()
    {
        Debug.Log("Mouse up");

        UseItemOnInterface();
    }


    void UseItemOnInterface()
    {
        var holdingItem = HotbarManager.Instance.HoldingItem;
        if (holdingItem == null || holdingItem.ItemType != InterfaceData.InterfaceTool)
        {
            if (holdingItem != null)
            {
                Debug.Log($"holding wrong item | holding : {holdingItem.ItemType} - Required Item : {InterfaceData.InterfaceTool}");
            }
            else
                Debug.Log($"Not holding an item | holdingItem is null {holdingItem == null}");
            return;

        }

        Debug.Log("Passed check 1");

        if (PlayerFocus.Instance.Focused != InterfaceData.InterfaceType)
            return;

        Debug.Log($"Used Item on Interface {InterfaceData.name}");
    }
}
