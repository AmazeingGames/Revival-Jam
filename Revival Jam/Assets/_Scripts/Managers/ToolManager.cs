using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemAndAbilityManager;

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

    public System.Collections.ObjectModel.ReadOnlyCollection<ItemData> GetUsedTools() => usedTools.AsReadOnly();

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

                AudioManager.TriggerAudioClip(AudioManager.EventSounds.Ending, ArcadeSoundEmitter.Transform);
                break;

            case ItemsAndAbilities.Screwdriver:
                break;

            case ItemsAndAbilities.Wrench:
                if (ControlsManager.IsControlConnected(ControlsManager.Controls.Jump))
                {
                    Debug.Log("Jump is already connected");
                    return;
                }

                var wires = ControlsManager.Instance.Wires;
                var receptacles = ControlsManager.Instance.Receptacles;

                var unusedWire = wires.FirstOrDefault(w => w.ConnectedReceptacle == null);

                if (unusedWire == null)
                {
                    Debug.Log("No unused wire");
                    return;
                }

                var jumpReceptacle = receptacles.First(r => r.LinkedControl == ControlsManager.Controls.Jump);

                Debug.Log($"Enable Jump | Using wire: {unusedWire} | Using receptacle: {jumpReceptacle}");

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
