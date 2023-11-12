using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemAndAbilityManager;

public class ToolManager : MonoBehaviour
{
    private void OnEnable()
    {
        Interface.UseItem += HandleUseTool;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUseTool;
    }

    
    void HandleUseTool(ItemData toolData)
    {
        if (toolData == null)
        {
            Debug.Log("tool data is null");
            return;
        }

        Debug.Log($"Handled tool use {toolData.ItemType}");

        switch (toolData.ItemType)
        {
            case ItemsAndAbilities.Crowbar:
                StartCoroutine(WiringManager.SetWiringCabinet(isActive: true));
                break;

            case ItemsAndAbilities.Hammer:
                StartCoroutine(ArcadeQuad.SetCabinet(isActive: false));
                Debug.Log("Thank you for freeing me! MuHAHAHA");
                break;

            case ItemsAndAbilities.Screwdriver:
                break;

            case ItemsAndAbilities.Wrench:
                break;
        }
    }

    void HandleSpawnTool(ItemData toolData)
    {
        if (toolData == null)
            return;

        switch (toolData.ItemType)
        {
            case ItemsAndAbilities.Crowbar:
                break;

            case ItemsAndAbilities.Hammer:
                break;

            case ItemsAndAbilities.Screwdriver:
                break;

            case ItemsAndAbilities.Wrench:
                break;
        }
    }

}
