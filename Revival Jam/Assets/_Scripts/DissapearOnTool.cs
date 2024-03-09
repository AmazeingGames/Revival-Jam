using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static ItemAndAbilityManager;

public class DissapearOnTool : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [Header("Appear Settings")]
    [SerializeField] bool appearOnToolGain = false;
    [SerializeField] bool appearOnToolUse = false;

    [SerializeField] Tools appearToolGainType;
    [SerializeField] Tools appearToolUseType;

    [Header("Disappear Settings")]
    [SerializeField] bool disappearOnToolGain = false;
    [SerializeField] bool disappearOnToolUse = false;

    [FormerlySerializedAs("toolGainType")]
    [SerializeField] Tools disappearToolGainType;
    [FormerlySerializedAs("toolUseType")]
    [SerializeField] Tools disappearToolUseType;

    private void OnEnable()
    {
        SpawnTool += HandleToolGain;
        Interface.UseItem += HandleToolUse;
    }
    private void OnDisable()
    {
        SpawnTool -= HandleToolGain;
        Interface.UseItem -= HandleToolUse;

    }

    // Start is called before the first frame update
    void Start()
    {
        CheckUsedToolsDisappear();
        CheckGainedToolsDisappear();
    }

    
    void HandleToolGain(Tools tool)
    {
        if (disappearOnToolGain)
        {
            if (tool != disappearToolGainType)
                return;

            gameObject.SetActive(false);
        }

        if (appearOnToolGain)
        {
            if (tool != appearToolGainType)
                return;

            gameObject.SetActive(true);
        }
    }

    //This whole disappear/appear thing needs to be refactored
    //Setup is a bit unintuitive/messy
    void HandleToolUse(ItemData itemData)
    {
        if (disappearOnToolUse)
        {
            if (itemData.ItemType != disappearToolUseType)
                return;

            Disappear();
        }

        if (appearOnToolUse)
        {
            if (itemData.ItemType != appearToolUseType)
                return;

            Appear();
        }
    }

    void CheckUsedToolsDisappear()
    {
        var usedTools = ToolManager.Instance.GetUsedTools();

        var usedToolTypes = from tool in usedTools select tool.ItemType;

        if (usedToolTypes.Contains(disappearToolUseType))
            Disappear();
    }

    void CheckGainedToolsDisappear()
    {
        var usedTools = HotbarManager.Instance.GetGainedTools();

        var usedToolTypes = from tool in usedTools select tool.ItemType;

        if (usedToolTypes.Contains(disappearToolGainType))
            Disappear();
    }

    void Disappear()
    {
        gameObject.SetActive(false);
    }
    
    void Appear()
    {
        gameObject.SetActive(true);
    }
}
