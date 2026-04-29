using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    public Animator anim;

    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmours;

    public static event Action<ShopManager, bool> OnShopStateChanged;
    private bool playerInRange;
    bool isopen;

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                isopen = !isopen;
                UiManager.Instance.ToggleUI(UIType.Shop);

                currentShopKeeper = this;
                OnShopStateChanged.Invoke(ShopManager.Instance, isopen);

                OpenItemShop();
            }
            else if(Input.GetButtonDown("Cancel"))
            {
                isopen = false;
                currentShopKeeper = null;
                OnShopStateChanged.Invoke(ShopManager.Instance, isopen);
            }
        }
    }

    public void OpenItemShop()
    {
        ShopManager.Instance.PopulateShopItems(shopItems);
    }
    public void OpenWeaponShop()
    {
        ShopManager.Instance.PopulateShopItems(shopWeapons);
    }
    public void OpenArmourShop()
    {
        ShopManager.Instance.PopulateShopItems(shopArmours);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
