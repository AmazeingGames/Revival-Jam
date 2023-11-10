using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] HorizontalLayoutGroup HotbarLayoutGroup;
    [SerializeField] GameObject slot;
    [SerializeField] Item item;

    [SerializeField] List<ItemData> itemsData;

    public Dictionary<ItemAndAbilityManager.ItemsAndAbilities, Item> ItemsDictionary = new();
    public List<Item> HotbarItems = new();

    private void OnEnable()
    {
        ItemAndAbilityManager.AbilityGain += HandleAbilityGain;
    }

    private void OnDisable()
    {
        ItemAndAbilityManager.AbilityGain -= HandleAbilityGain;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Instantiates the hotbar essentially
        //Readies items and their respective data
        foreach (var itemData in itemsData)
        {
            Transform currentSlot = Instantiate(slot).transform;

            currentSlot.SetParent(HotbarLayoutGroup.transform);

            Item currentItem = Instantiate(item);

            currentItem.InitializeData(currentSlot, itemData);

            HotbarItems.Add(currentItem);
            ItemsDictionary.Add(currentItem.ItemData.ItemType, currentItem);

            currentItem.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DebugGainItems();
    }

    //Responds to gaining an ability be activating the given item for use
    void HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities ability)
    {
        if (ItemsDictionary.ContainsKey(ability))
            ItemsDictionary[ability].gameObject.SetActive(true);
    }

    //Simulates what it would be like to handle an Ability Gain
    void DebugGainItems()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.I))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Crowbar);
        if (Input.GetKeyDown(KeyCode.O))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Hammer);
        if (Input.GetKeyDown(KeyCode.P))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Wrench);
#endif
    }

   
}
