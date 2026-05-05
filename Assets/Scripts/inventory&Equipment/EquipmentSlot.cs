using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : ItemSlot, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Image itemImage;

    public ShopPopup popup;
    private static ShopManager activeShop;
    public GameObject selectBorder;

    [SerializeField] private itemInfo info;

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
                InventoryManager.Instance.DeselectItem();
            }
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.clickCount >= 2)
            {
                info.HideItemInfo();
            }

            if (activeShop != null)
            {
                popup.OpenPopup(itemSO, ShopMode.Sell, itemSO.saleprice,InventoryManager.Instance.gold, quantity);
                UpdateUI();
            }
            else
            {
                InventoryManager.Instance.OnSlotClicked(this, eventData.clickCount);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.Instance.DropItem(this);
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
        }
        else
        {
            itemImage.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) //드래그를 시작할때
    {
        if (activeShop != null) return;
        if (itemSO == null) return;

        InventoryManager.Instance.dragIcon.sprite = itemSO.icon; //드래그 이미지 생성
        InventoryManager.Instance.dragIcon.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData) //드래그 중일때
    {
        if (activeShop != null) return;

        InventoryManager.Instance.dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) //드래그가 끝날때
    {
        if (activeShop != null) return;

        InventoryManager.Instance.dragIcon.gameObject.SetActive(false);
        itemImage.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        EquipmentSlot draggedSlot = eventData.pointerDrag.GetComponent<EquipmentSlot>();

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
            InventoryManager.Instance.DeselectItem();
            InventoryManager.Instance.OnSlotClicked(this, 1);
        }
        else
        {
            //두개 다 아이템이면 교환
            InventoryManager.Instance.SwapItems(draggedSlot, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
