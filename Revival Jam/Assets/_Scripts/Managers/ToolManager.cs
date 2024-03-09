using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ItemAndAbilityManager;
using System.Collections.ObjectModel;

public class ToolManager : Singleton<ToolManager>
{
    private void OnEnable()
    {
        Interface.UseItem += HandleUseTool;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUseTool;
    }

    readonly List<ItemData> usedTools = new();  

    public ReadOnlyCollection<ItemData> GetUsedTools() => usedTools.AsReadOnly();
    public IEnumerable<ItemsAndAbilities> GetUsedToolsTypes() => usedTools.Select(t => t.ItemType);
    public bool HasUsedTool(ItemsAndAbilities toolType) => GetUsedToolsTypes().Contains(toolType);

    void HandleUseTool(ItemData toolData)
    {
        if (toolData == null)
        {
            Debug.Log("toolType data is null");
            return;
        }

        Debug.Log($"Handled toolType use {toolData.ItemType}");

        switch (toolData.ItemType)
        {
            // Allows use of control wiring
            case ItemsAndAbilities.Crowbar:
                StartCoroutine(WiringManager.SetWiringCabinet(isActive: true));
                break;

            // Ends the game
            case ItemsAndAbilities.Hammer:
                StartCoroutine(ArcadeQuad.SetCabinet(isActive: false));
                GameManager.Instance.UpdateGameState(GameManager.GameState.EndGame);
                break;

            // Allows the player to jump
            case ItemsAndAbilities.Wrench:
                if (ControlsManager.IsControlConnected(ControlsManager.Controls.Jump))
                {
                    Debug.Log("Jump is already connected");
                    return;
                }

                // Finds a wire that's not connected to a control
                var wires = ControlsManager.Instance.Wires;
                var unusedWire = wires.FirstOrDefault(w => w.ConnectedReceptacle == null);
                if (unusedWire == null)
                {
                    Debug.LogWarning("All wires in use");
                    return;
                }

                // Find the receptacle connected to the jump control
                var receptacles = ControlsManager.Instance.Receptacles;
                var jumpReceptacle = receptacles.First(r => r.LinkedControl == ControlsManager.Controls.Jump);
                
                // Connect jump
                unusedWire.ManuallyConnect(jumpReceptacle);
                break;

            default:
                Debug.LogWarning("Case not covered");
                return;
        }

        usedTools.Add(toolData);
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
