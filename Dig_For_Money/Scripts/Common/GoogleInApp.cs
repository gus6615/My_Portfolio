using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class GoogleInApp : MonoBehaviour, IStoreListener
{
    public static GoogleInApp instance;
    private const string package = "package";
    private const string package_ID = "package";
    private const string removeAD = "removeAD"; // 스크립트 임시 ID
    private const string removeAD_ID = "remove_ad"; // 구글 실제 상품 ID
    private string[] red_diamonds = { "red_diamond_100", "red_diamond_350", "red_diamond_800", "red_diamond_1800" }; // 스크립트 임시 ID
    private string[] red_diamond_IDs = { "red_diamond_100", "red_diamond_350", "red_diamond_800", "red_diamond_1800" }; // 구글 실제 상품 ID

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    public bool isInitialized;
    private bool isIniting;

    void Awake()
    {
        if(instance == null)
        {
            Init();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !isIniting)
            StartCoroutine("InitialCoroutine");
    }

    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(package, ProductType.NonConsumable, new IDs() { { package_ID, GooglePlay.Name } });
        builder.AddProduct(removeAD, ProductType.NonConsumable, new IDs() {{ removeAD_ID, GooglePlay.Name }});
        for (int i = 0; i < red_diamond_IDs.Length; i++)
            builder.AddProduct(red_diamonds[i], ProductType.Consumable, new IDs() { { red_diamond_IDs[i], GooglePlay.Name } });

        UnityPurchasing.Initialize(this, builder);
    }

    IEnumerator InitialCoroutine()
    {
        isIniting = true;
        yield return new WaitForSeconds(5f);
        Init();
        isIniting = true;
    }

    public void PurchaseTest()
    {
        Debug.Log("개발자 전용 상품 구매 요청");
        if (!instance.HadPurchased(removeAD))
        {
            Debug.Log("구매하지 않은 상품");
            instance.Purchase(removeAD);
        }
        else
        {
            Debug.Log("이미 구매한 상품");
        }
    }

    public void Purchase_Package()
    {
        if (instance.HadPurchased(package))
        {
            Debug.Log("이미 구매한 상품: '패키지'");
            return;
        }
        instance.Purchase(package);
    }

    public void Purchase_RemoveAD()
    {
        if (instance.HadPurchased(removeAD))
        {
            Debug.Log("이미 구매한 상품: '광고 제거'");
            return;
        }
        instance.Purchase(removeAD);
    }

    public void Purchase_RedDiamond(int index)
    {
        string id = red_diamonds[index];

        if (instance.HadPurchased(id))
        {
            Debug.Log("이미 구매한 상품: '레드 다이아몬드' [" + index + "]번째 상품");
            return;
        }
        instance.Purchase(id);
    }

    public void Purchase(string productID)
    {
        if (!isInitialized) return;

        var product = storeController.products.WithID(productID);
        
        if(product != null && product.availableToPurchase)
        {
            Debug.Log("구매 시도 : " + product.definition.id);
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log("구매 시도 불가 : " + productID);
        }

        // 구글 플레이가 아닌, 아이폰 등 다른 플랫폼은 따로 처리가 필요
    }

    public bool HadPurchased(string productID)
    {
        if (!isInitialized) return false;

        var product = storeController.products.WithID(productID);

        if(product != null && product.definition.type == ProductType.NonConsumable)
        {
            return product.hasReceipt;
        }

        return false;
    }

    public bool HadPurchased_removeAD()
    {
        if (!isInitialized) return false;

        var product = storeController.products.WithID(removeAD);

        if (product != null && product.definition.type == ProductType.NonConsumable)
        {
            return product.hasReceipt;
        }

        return false;
    }

    public bool HadPurchased_package()
    {
        if (!isInitialized) return false;

        var product = storeController.products.WithID(package);

        if (product != null && product.definition.type == ProductType.NonConsumable)
        {
            return product.hasReceipt;
        }

        return false;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        isInitialized = true;
        SaveScript.saveData.isRemoveAD = HadPurchased_removeAD() || HadPurchased_package();
        Debug.Log("광고 상태: " + HadPurchased_removeAD() + " / " + "패키지 상태: " + HadPurchased_package());
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("유니티 인앱 결제 IAP 초기화 실패 : " + error);
        isInitialized = false;
        SaveScript.saveData.isRemoveAD = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("유니티 인앱 결제 IAP 초기화 실패 : " + error + "\nMessage : " + message);
        isInitialized = false;
        SaveScript.saveData.isRemoveAD = false;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("구매 성공 - 상품 ID :" + args.purchasedProduct.definition.id);

        if (args.purchasedProduct.definition.id == removeAD)
        {
            Debug.Log("구매 성공 - 광고 제거 상품");
            SaveScript.saveData.isRemoveAD = true;
            if (Shop.instance != null)
            {
                Shop.instance.SetBasicInfo();
                Shop.instance.SetAudio(4);
            }
            if (CashItemShop.instance != null)
            {
                CashItemShop.instance.OnMenuButton();
                CashItemShop.instance.SetRemoveADSelect();
            }
        }
        else if (args.purchasedProduct.definition.id == package)
        {
            Debug.Log("구매 성공 - 패키지 상품");
            SaveScript.saveData.isRemoveAD = true;
            SaveScript.saveData.cash += CashItemShop.cashes[2];
            AchievementCtrl.instance.SetAchievementAmount(24, CashItemShop.cashes[2]);
            if (Shop.instance != null)
            {
                Shop.instance.SetBasicInfo();
                Shop.instance.SetAudio(4);
            }
            if (CashChargeShop.instance != null)
                CashChargeShop.instance.SetContent();
        }
        else
        {
            Debug.Log("구매 성공 - 레드 다이아몬드 상품");
            if (CashChargeShop.chargeIndex == -1)
            {
                Debug.LogError("Error: No Selected Red Diamond Product");
            }
            else
            {
                if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 5)
                    SaveScript.saveData.cash += (int)(CashItemShop.cashes[CashChargeShop.chargeIndex] * 1.5f);
                else
                    SaveScript.saveData.cash += CashItemShop.cashes[CashChargeShop.chargeIndex];
                AchievementCtrl.instance.SetAchievementAmount(24, CashItemShop.cashes[CashChargeShop.chargeIndex]);
                if (Shop.instance != null)
                    Shop.instance.SetBasicInfo();
            }
        }
        SaveScript.SaveData_Syn();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("구매 실패 - 상품 ID :" + product.definition.id + " / 원인 : " + failureReason);
    }
}
