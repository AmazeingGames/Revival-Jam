using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interface : MonoBehaviour
{
    [SerializeField] InterfaceData InterfaceData;

    private void OnMouseDown()
    {
        Debug.Log("Mouse click!");

        UseItemOnInterface();
    }


    void UseItemOnInterface()
    {
        if (HotbarManager.Instance.HoldingItem.ItemType != InterfaceData.InterfaceTool)
            return;

        Debug.Log("Passed check 1");

        if (PlayerFocus.Instance.Focused != InterfaceData.InterfaceType)
            return;

        Debug.Log("Used Item on Interface");
    }
}
