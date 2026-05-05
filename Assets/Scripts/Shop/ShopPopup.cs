using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPopup : MonoBehaviour
{
    ShopMode currentMode;
    ItemSO currentItem;

    int currentAmount = 1;
    int maxAmount;
    int totalPrice;
    int playerGold;
    int unitPrice; //슬롯 구매가
    private bool isProcessing = false; //중복클릭 방지용

    public UIAnim uianim;

    public GameObject Popup;

    public Slider amountSlider;

    public TMP_Text description;
    public TMP_Text amountText;
    public TMP_Text priceText;
    public TMP_Text ComfirmText;

    public Button minusButton;
    public Button plusButton;
    public Button confirmButton;

    public Color normalColor = Color.black;
    public Color warningColor = Color.red;

    public void OpenPopup(ItemSO item, ShopMode mode, int price, int playerGold, int playerItemCount)
    {
        isProcessing = false;
        uianim.Show(Popup);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        currentItem = item;
        currentMode = mode;
        unitPrice = price;
        this.playerGold = playerGold;

        if (mode == ShopMode.Buy)
        {
            description.text = "구매할 수량을 입력해 주세요";
            ComfirmText.text = "구매";
            maxAmount = playerGold / price; //돈 기준
        }
        else
        {
            description.text = "판매할 수량을 입력해 주세요";
            ComfirmText.text = "판매";
            maxAmount = playerItemCount; //보유 개수 기준
        }

        maxAmount = Mathf.Max(1, maxAmount); //최소값 1

        currentAmount = 1;

        amountSlider.minValue = 1;
        amountSlider.maxValue = maxAmount;
        amountSlider.value = 1;

        UpdateUI();
    }

    void UpdateUI()
    {
        if(currentMode == ShopMode.Buy)
        {
            totalPrice = unitPrice * currentAmount;
        }
        else
        {
            totalPrice = currentItem.saleprice * currentAmount;
        }


        amountText.text = currentAmount.ToString();
        priceText.text = totalPrice.ToString();

        //버튼 활성/비활성화
        minusButton.interactable = currentAmount > 1;
        plusButton.interactable = currentAmount < maxAmount;

        if (currentMode == ShopMode.Buy)
        {
            bool canBuy = playerGold >= totalPrice;

            confirmButton.interactable = canBuy;

            priceText.color = canBuy ? normalColor : warningColor;
        }
        else
        {
            confirmButton.interactable = true;
            priceText.color = normalColor;
        }
    }

    public void OnClickPlus() //수량 플러스 버튼
    {
        amountSlider.value++;
    }  

    public void OnClickMinus() //수량 마이너스 버튼
    {
        amountSlider.value--;
    }

    public void OnClickMax()
    {
        currentAmount = maxAmount;
        amountSlider.value = maxAmount;
        UpdateUI();
    }

    public void OnConfirm()
    {
        if (isProcessing) return;

        isProcessing = true;

        if (currentMode == ShopMode.Buy)
        {
            ShopManager.Instance.TryBuyItem(currentItem, unitPrice, currentAmount);
        }
        else
        {
            ShopManager.Instance.SellItem(currentItem, currentAmount);
        }

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cash);
        uianim.Hide(Popup);
    }

    public void OnSliderChanged(float value)
    {
        currentAmount = Mathf.RoundToInt(value);
        UpdateUI();
        //Debug.Log("슬라이더 값: " + value);
    }

    public void ClosePopup()
    {
        uianim.Hide(Popup);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        UpdateUI();
    }
}

public enum ShopMode
{
    Buy,
    Sell
}
