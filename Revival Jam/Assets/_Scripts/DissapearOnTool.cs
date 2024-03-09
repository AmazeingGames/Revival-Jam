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

    [SerializeField] ItemsAndAbilities appearToolGainType;
    [SerializeField] ItemsAndAbilities appearToolUseType;

    [Header("Disappear Settings")]
    [SerializeField] bool disappearOnToolGain = false;
    [SerializeField] bool disappearOnToolUse = false;

    [FormerlySerializedAs("toolGainType")]
    [SerializeField] ItemsAndAbilities disappearToolGainType;
    [FormerlySerializedAs("toolUseType")]
    [SerializeField] ItemsAndAbilities disappearToolUseType;

    private void OnEnable()
    {
        ItemAndAbilityManager.GainAbility += HandleAbilityGain;
        Interface.UseItem += HandleToolUse;
    }
    private void OnDisable()
    {
        ItemAndAbilityManager.GainAbility -= HandleAbilityGain;
        Interface.UseItem -= HandleToolUse;

    }

    // Start is called before the first frame update
    void Start()
    {
        CheckUsedToolsDisappear();
        CheckGainedToolsDisappear();
    }

    void HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities abilities)
    {
        if (disappearOnToolGain)
        {
            if (abilities != disappearToolGainType)
                return;

            gameObject.SetActive(false);
        }

        if (appearOnToolGain)
        {
            if (abilities != appearToolGainType)
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


    void SetActive(bool active)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
