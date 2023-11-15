using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DissapearOnTool : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [Header("Appear Settings")]
    [SerializeField] bool appearOnToolGain = false;
    [SerializeField] bool appearOnToolUse = false;

    [SerializeField] ItemAndAbilityManager.ItemsAndAbilities appearToolGainType;
    [SerializeField] ItemAndAbilityManager.ItemsAndAbilities appearToolUseType;

    [Header("Disappear Settings")]
    [SerializeField] bool disappearOnToolGain = false;
    [SerializeField] bool disappearOnToolUse = false;

    [SerializeField] ItemAndAbilityManager.ItemsAndAbilities toolGainType;
    [SerializeField] ItemAndAbilityManager.ItemsAndAbilities toolUseType;

    private void OnEnable()
    {
        ItemAndAbilityManager.AbilityGain += HandleAbilityGain;
        Interface.UseItem += HandleToolUse;
    }
    private void OnDisable()
    {
        ItemAndAbilityManager.AbilityGain -= HandleAbilityGain;
        Interface.UseItem -= HandleToolUse;

    }

    // Start is called before the first frame update
    void Start()
    {
           
    }

    void HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities abilities)
    {
        if (disappearOnToolGain)
        {
            if (abilities != toolGainType)
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

    void HandleToolUse(ItemData itemData)
    {
        if (disappearOnToolUse)
        {
            if (itemData.ItemType != toolUseType)
                return;

            gameObject.SetActive(false);
        }

        if (appearOnToolUse)
        {
            if (itemData.ItemType != appearToolUseType)
                return;

            gameObject.SetActive(true);
        }
    }

    void SetActive(bool active)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
