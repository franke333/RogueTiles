using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotDisplayer : MonoBehaviour
{
    private ItemSlot slot;

    public ItemSlot Slot { get => slot; }
    public Image itemSpriteImage;
    public Text itemName;
    public Text itemType;

    public void Link(ItemSlot slot)
    {
        this.slot = slot;
    }

    public void Display()
    {
        if (slot == null)
            return;
        itemType.text = slot.slotType.ToString();
        if (slot.IsEmpty)
            return;
        itemSpriteImage.sprite = slot.item.sprite;
        itemName.text = slot.item.itemName;
        
    }

    private void OnEnable()
    {
        Display();
    }
}
