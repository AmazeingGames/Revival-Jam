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
        ItemSpawner.GetSpawnReference += HandleSpawnerReference;
        ItemAndAbilityManager.SpawnTool += HandleToolSpawn;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUseTool;
        ItemAndAbilityManager.SpawnTool -= HandleToolSpawn;
        ItemSpawner.GetSpawnReference += HandleSpawnerReference;
    }


    readonly List<ItemData> usedTools = new();
    readonly Dictionary<Tools, ItemSpawner> toolTypeToSpawner = new();

    public ReadOnlyCollection<ItemData> GetUsedTools() => usedTools.AsReadOnly();
    public IEnumerable<Tools> GetUsedToolsTypes() => usedTools.Select(t => t.ItemType);
    public bool HasUsedTool(Tools toolType) => GetUsedToolsTypes().Contains(toolType);

    void HandleSpawnerReference(ItemSpawner itemSpawner)
        => toolTypeToSpawner.Add(itemSpawner.ToolData.ItemType, itemSpawner);


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
            case Tools.Crowbar:
                StartCoroutine(WiringManager.SetWiringCabinet(isActive: true));
                break;

            // Ends the game
            case Tools.Hammer:
                StartCoroutine(ArcadeQuad.SetCabinet(isActive: false));
                GameManager.Instance.UpdateGameState(GameManager.GameState.EndGame);
                break;

            // Allows the player to jump
            case Tools.Wrench:
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

    void HandleToolSpawn(Tools tool)
    {
        if (toolTypeToSpawner.TryGetValue(tool, out ItemSpawner spawner))
            spawner.SpawnItem();
    }

}
