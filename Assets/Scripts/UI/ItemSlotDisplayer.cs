using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotDisplayer : MonoBehaviour, IPointerDownHandler
{
    private ItemSlot slot;

    public ItemSlot Slot { get => slot; }
    public Image itemSpriteImage;
    public Text itemName;
    public Text itemType;

    [SerializeField]
    private Image _background;

    private Color _baseBackgroundColor;
    private Color _highlightColor = Color.yellow;

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
        if(_background == null)
            _background = GetComponent<Image>();
        _baseBackgroundColor = _background.color;
    }

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
