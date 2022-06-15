using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// more like inventory manager
public class InventoryDisplayer : SingletonClass<InventoryDisplayer>
{
    public GameObject inventory;
    public List<ItemSlotDisplayer> slotDisplayers;
    [SerializeField]
    private List<GameObject> slotSwitchButtons;
    [SerializeField]
    private GameObject pickUpDialogGO;


    private ItemSlot pickedUpItemSlot;
    [SerializeField]
    private ItemSlotDisplayer pickedUpItemSlotDisplayer;

    private void Start()
    {
        inventory.gameObject.SetActive(false);
        pickedUpItemSlot = new ItemSlot();
        pickedUpItemSlotDisplayer.Link(pickedUpItemSlot);
        
    }
    // Update is called once per frame
    void Update()
    {
        // if inventory dialog is up -> we need the player to choose what to di with item first
        if (pickUpDialogGO.activeInHierarchy)
            return;
        if (Input.GetKeyDown(KeyCode.I))
            inventory.gameObject.SetActive(!inventory.activeSelf);
    }

    public bool InventoryWindowActive => inventory.gameObject.activeSelf;

    public void DisplayPickUpDialog(Item pickedUpItem)
    {
        if (pickedUpItem == null)
            return;
        pickedUpItemSlot.SetItem(pickedUpItem);
        pickedUpItemSlotDisplayer.Display();
        for (int i = 0; i < slotDisplayers.Count; i++)
           slotSwitchButtons[i].SetActive(slotDisplayers[i].Slot.slotType == pickedUpItemSlot.slotType ||
                                          slotDisplayers[i].Slot.slotType == ItemType.Any);
        pickUpDialogGO.SetActive(true);
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
}
