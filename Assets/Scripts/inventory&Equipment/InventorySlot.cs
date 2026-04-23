using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : ItemSlot, IPointerClickHandler, 
    IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler, 
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Image itemImage;
    public TMP_Text quantityText;

    public ShopPopup popup;
    public InventoryManager inventoryManager;
    private static ShopManager activeShop;
    public GameObject selectBorder;

    [SerializeField] private itemInfo info;
    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopStateChanged += HandleShopStateChanged;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopStateChanged -= HandleShopStateChanged;
    }
    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity <= 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventoryManager.DeselectItem();
            }
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(activeShop != null)
            {
                popup.OpenPopup(itemSO, ShopMode.Sell, itemSO.saleprice, InventoryManager.Instance.gold, quantity);
                //Debug.Log(quantity);
            }
            else
            {
                inventoryManager.OnSlotClicked(this, eventData.clickCount);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryManager.DropItem(this);
        }
    }

    public override void UpdateUI()
    {
        if (quantity <= 0)
            itemSO = null;

        if (itemSO != null)
        {
            itemImage.sprite = itemSO.icon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }

    public void OnBeginDrag(PointerEventData eventData) //드래그를 시작할때
    {
        if (itemSO == null) return;

        inventoryManager.dragIcon.sprite = itemSO.icon; //드래그 이미지 생성
        inventoryManager.dragIcon.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData) //드래그 중일때
    {
        inventoryManager.dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) //드래그가 끝날때
    {
        inventoryManager.dragIcon.gameObject.SetActive(false);
        itemImage.enabled = true;
        quantityText.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot == null || draggedSlot == this)
            return;

        // 빈슬롯을 드래그했을때
        if (draggedSlot.itemSO == null)
            return;

        // 아이템을 빈슬롯으로 드래그했을때
        if (itemSO == null)
        {
            itemSO = draggedSlot.itemSO;
            quantity = draggedSlot.quantity;

            draggedSlot.itemSO = null;
            draggedSlot.quantity = 0;

            UpdateUI();
            draggedSlot.UpdateUI();
            inventoryManager.DeselectItem();
            inventoryManager.OnSlotClicked(this, 1);
        }
        else
        {
            //두개 다 아이템이면 교환
            inventoryManager.SwapItems(draggedSlot, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (SkillMessageUI.Instance.isOpen) return;

        if (itemSO == null) return;

        info.ShowItemInfo(itemSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        info.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (info != null) info.FollowMouse();
    }

    public override void Select()
    {
        selectBorder.SetActive(true);
    }

    public override void Deselect()
    {
        selectBorder.SetActive(false);
    }

    public void Set(ItemSO item, int count)
    {
        itemSO = item;
        quantity = count;

        UpdateUI();
    }

    public void Clear()
    {
        itemSO = null;
        quantity = 0;

        UpdateUI();
    }
}
