using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : Singleton<HotbarManager>
{
    [SerializeField] HorizontalLayoutGroup HotbarLayoutGroup;
    [SerializeField] GameObject slot;
    [SerializeField] Item item;

    [SerializeField] List<ItemData> itemsData;

    readonly Dictionary<ItemAndAbilityManager.ItemsAndAbilities, Item> ItemsDictionary = new();
    readonly List<Item> HotbarItems = new();

    public ItemData HoldingItem { get; private set; } = null;

    private void OnEnable()
    {
        ItemAndAbilityManager.AbilityGain += HandleAbilityGain;
        Item.GrabTool += HandleGrabTool;
    }

    private void OnDisable()
    {
        ItemAndAbilityManager.AbilityGain -= HandleAbilityGain;
        Item.GrabTool -= HandleGrabTool;
    }

    void HandleGrabTool(bool isGrabbingTool, ItemData data)
    {
        if (isGrabbingTool)
            HoldingItem = data;
        else
            StartCoroutine(DropHoldingItem());
    }

    //Gives a delay for the interface to know we're holding an item
    IEnumerator DropHoldingItem()
    {
        yield return new WaitForSeconds(.1f);

        HoldingItem = null;
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

    //Adds the item to the player's hotbar
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
