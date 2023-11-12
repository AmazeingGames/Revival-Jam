using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemAndAbilityManager;

public class ToolManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void HandleUseTool(ItemData toolData)
    {
        if (toolData == null)
            return;

        switch (toolData.ItemType)
        {
            case ItemsAndAbilities.Crowbar:
                StartCoroutine(WiringManager.SetWiringCabinet(isActive: false));
                break;

            case ItemsAndAbilities.Hammer:
                Debug.Log("Thank you for freeing me! MuHAHAHA");
                break;

            case ItemsAndAbilities.Screwdriver:
                break;

            case ItemsAndAbilities.Wrench:
                break;
        }
    }
}
