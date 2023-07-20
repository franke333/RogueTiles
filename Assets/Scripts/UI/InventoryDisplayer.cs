using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// more like inventory manager

/// <summary>
/// Class that manages the inventory UI
/// </summary>
public class InventoryDisplayer : SingletonClass<InventoryDisplayer>
{
    public GameObject inventory;
    public List<ItemSlotDisplayer> slotDisplayers;
    [SerializeField]
    private List<GameObject> slotSwitchButtons;
    [SerializeField]
    private GameObject pickUpDialogGO;

    private ItemSlotDisplayer _lastHighlightedSlotDisplayer;

    private ItemSlot pickedUpItemSlot;
    [SerializeField]
    private ItemSlotDisplayer pickedUpItemSlotDisplayer;
    [SerializeField]
    private List<CardDisplayer> _inventoryItemCardDisplayers;

    private void Start()
    {
        inventory.gameObject.SetActive(false);
        pickedUpItemSlot = new ItemSlot();
        pickedUpItemSlotDisplayer.Link(pickedUpItemSlot);
        HideAllInventoryItemCards();
        
    }
    // Update is called once per frame
    void Update()
    {
        // if inventory dialog is up -> we need the player to choose what to do with item first
        if (pickUpDialogGO.activeInHierarchy)
            return;
        if (Input.GetKeyDown(KeyCode.I))
            inventory.gameObject.SetActive(!inventory.activeSelf);
    }

    public bool InventoryWindowActive => inventory.gameObject.activeSelf;


    /// <summary>
    /// Displays the pick up dialog for the given item
    /// </summary>
    /// <param name="pickedUpItem">Picked up Item</param>
    public void DisplayPickUpDialog(Item pickedUpItem)
    {
        if (pickedUpItem == null)
            return;
        if (pickedUpItem.itemType == ItemType.Consumable)
        {
            ((ConsumableItem)pickedUpItem).HealPlayer();
            return;
        }
        pickedUpItemSlot.SetItem(pickedUpItem);
        
        pickedUpItemSlotDisplayer.Display();
        for (int i = 0; i < slotDisplayers.Count; i++)
            slotSwitchButtons[i].gameObject.SetActive((slotDisplayers[i].Slot.slotType == pickedUpItemSlot.item.itemType) ||
                                           (slotDisplayers[i].Slot.slotType == ItemType.Any));

        pickUpDialogGO.SetActive(true);
        inventory.gameObject.SetActive(true);
        DisplayCardsOfItemInItemSlot(pickedUpItemSlotDisplayer);
        pickedUpItemSlotDisplayer.itemType.text = pickedUpItem.itemType.ToString();
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.PickUp);
    }

    public void SwitchItemWith(int slotIndex)
    {
        slotDisplayers[slotIndex].Slot.SetItem(pickedUpItemSlot.item);
        pickedUpItemSlot.SetItem(null);
        pickUpDialogGO.SetActive(false);
        inventory.gameObject.SetActive(false);
    }

    public void ThrowAwayPickedUpItem()
    {
        pickedUpItemSlot.SetItem(null);
        pickUpDialogGO.SetActive(false);
        inventory.gameObject.SetActive(false);
    }

    public void HideAllInventoryItemCards()
    {
        foreach (var item in _inventoryItemCardDisplayers)
            item.Hide();
    }

    public void DisplayCardsOfItemInItemSlot(ItemSlotDisplayer itemSlotDisplayer)
    {
        //highlight colors in inventory
        if (_lastHighlightedSlotDisplayer != null)
            _lastHighlightedSlotDisplayer.SetHighlight(false);
        _lastHighlightedSlotDisplayer = itemSlotDisplayer;
        itemSlotDisplayer.SetHighlight(true);
        HideAllInventoryItemCards();
        if (itemSlotDisplayer.Slot.IsEmpty)
        {
            return;
        }

        //display cards
        Item item = itemSlotDisplayer.Slot.item;
        for (int i = 0; i < item.cards.Count; i++)
        {
            _inventoryItemCardDisplayers[i].DisplayCard(item.cards[i]);
        }
    }
}
