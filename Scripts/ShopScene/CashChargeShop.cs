using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CashChargeShop : MonoBehaviour
{
    static public int chargeIndex = -100;
    static public CashChargeShop instance;
    static private string[] slot_infos =
    {
        "",
        "450(기본) + 150(보너스)",
        "900(기본) + 600(보너스)",
        "1800(기본) + 1700(보너스)",
    };

    [SerializeField] private UIBox package_uibox;
    [SerializeField] private GameObject eventObject;
    [SerializeField] private UIBox[] slots;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetContent();
    }

    public void ChargeButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>() != null)
        {
            chargeIndex = EventSystem.current.currentSelectedGameObject.GetComponent<UIBox>().order;
            Shop.instance.SetAudio(0);
        }

        // 예외 처리
        if (chargeIndex == -100)
        {
            Debug.LogError("Cash Charge Error!");
            return;
        }

        if (chargeIndex == -1)
            // 패키지
            GoogleInApp.instance.Purchase_Package();
        else // 캐시 충전
            GoogleInApp.instance.Purchase_RedDiamond(chargeIndex);
    }

    public void SetContent()
    {
        if (GoogleInApp.instance.HadPurchased_package())
        {
            // 이미 구해 함
            package_uibox.images[0].color = Color.white * 0.4f;
            package_uibox.images[1].color = package_uibox.images[2].color = Color.white * 0.2f;
            package_uibox.texts[0].color = new Color(0.8f, 0.8f, 0.8f, 0.2f);
            package_uibox.texts[1].color = new Color(0.8f, 0.8f, 0.8f, 0.1f);
            package_uibox.texts[2].color = new Color(0.8f, 0.8f, 0.8f, 0.2f);
            package_uibox.button.enabled = false;
        }
        else
        {
            // 구매 가능
            package_uibox.images[0].color = new Color(0f, 0.6f, 1f);
            package_uibox.images[1].color = package_uibox.images[2].color = Color.white;
            package_uibox.texts[0].color = new Color(0.4f, 0.5f, 1f, 0.8f);
            package_uibox.texts[1].color = new Color(0.8f, 0.8f, 1f, 0.8f);
            package_uibox.texts[2].color = new Color(0.2f, 0.2f, 0.5f);
            package_uibox.button.enabled = true;
        }

        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 5)
        {
            eventObject.SetActive(true);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].texts[0].color = new Color(0.6f, 0.8f, 1f);
                slots[i].texts[0].text = (CashItemShop.cashes[i] * 1.5f).ToString();
                slots[i].texts[1].text = "!! Event !!\n<" + CashItemShop.cashes[i] + " -> " + (CashItemShop.cashes[i] * 1.5f) + " >";
            }
        }
        else
        {
            eventObject.SetActive(false);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].texts[0].color = new Color(1f, 0.6f, 0.6f);
                slots[i].texts[0].text = CashItemShop.cashes[i].ToString();
                slots[i].texts[1].text = slot_infos[i];
            }
        }
    }
}
