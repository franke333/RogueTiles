using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Item slot displayer that displays an item slot in inventory
/// </summary>
public class ItemSlotDisplayer : MonoBehaviour, IPointerDownHandler
{
    private ItemSlot slot;

    public ItemSlot Slot { get => slot; }
    public Image itemSpriteImage;
    public Text itemName;
    public Text itemType;

    private Image _background;

    private Color _baseBackgroundColor;
    private Color _highlightColor = Color.yellow;

    /// <summary>
    /// Link this displayer to a slot
    /// </summary>
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

    private void Start()
    {
        if (_background == null)
        {
            _background = GetComponent<Image>();
            _baseBackgroundColor = _background.color;
        }
    }

    /// <summary>
    /// Set the highlight of this item slot
    /// (used to display cards of the item in the item slot)
    /// </summary>
    public void SetHighlight(bool value)
    {
        if (_background == null)
        {
            _background = GetComponent<Image>();
            _baseBackgroundColor = _background.color;
        }
        if (!value)
            _background.color = _baseBackgroundColor;
        else
            _background.color = _highlightColor * _baseBackgroundColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!gameObject.activeInHierarchy)
            return;
        InventoryDisplayer.Instance.DisplayCardsOfItemInItemSlot(this);
    }
}
