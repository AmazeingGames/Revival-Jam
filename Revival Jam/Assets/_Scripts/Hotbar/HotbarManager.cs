using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    readonly List<ItemData> gainedTools = new(); //All gained tools
    readonly List<ItemData> currentTools = new(); //Tools currently in hotbar
    public System.Collections.ObjectModel.ReadOnlyCollection<ItemData> GetGainedTools() => gainedTools.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<ItemData> GetCurrentTools() => currentTools.AsReadOnly();


    private void OnEnable()
    {
        Interface.UseItem += HandleUseItem;

        ItemAndAbilityManager.GainAbility += HandleAbilityGain;
        Item.GrabTool += HandleGrabTool;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUseItem;

        ItemAndAbilityManager.GainAbility -= HandleAbilityGain;
        Item.GrabTool -= HandleGrabTool;
    }

    void HandleGrabTool(bool isGrabbingTool, ItemData data)
    {
        if (isGrabbingTool)
            HoldingItem = data;
        else
            StartCoroutine(DropHoldingItem());
    }

    // Gives a delay for the interface to know we're holding an itemToGain
    IEnumerator DropHoldingItem()
    {
        yield return new WaitForSeconds(.1f);

        HoldingItem = null;
    }
    void Start()
    {
        // Instantiates the hotbar and readies item data
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

    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.U))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Crowbar);
        if (Input.GetKeyDown(KeyCode.I))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Hammer);
        if (Input.GetKeyDown(KeyCode.O))
            HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities.Wrench);
#endif
    }

    void HandleUseItem(ItemData itemData)
        => currentTools.Remove(itemData);

    //Adds the itemToGain to the player's hotbar
    void HandleAbilityGain(ItemAndAbilityManager.ItemsAndAbilities ability)
    {
        if (!ItemsDictionary.ContainsKey(ability))
        {
            Debug.Log("Item doesn't exist!");
            return;
        }

        Item itemToGain = ItemsDictionary[ability];

        if (currentTools.Contains(itemToGain.ItemData))
        {
            Debug.Log("Player already has item!");
            return;
        }

        itemToGain.gameObject.SetActive(true);

        gainedTools.Add(itemToGain.ItemData);
        currentTools.Add(itemToGain.ItemData);
    }
}
