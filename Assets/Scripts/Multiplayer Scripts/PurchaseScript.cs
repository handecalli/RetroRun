using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab;
using UnityEngine.UI;

public class PurchaseScript : MonoBehaviour {

	public delegate void PurchaseAction(string key);
	public static event PurchaseAction updateMarketItems;
    public static event PurchaseAction OnError;
	
	public delegate void OnPurchaseAction();
	public static event OnPurchaseAction OnPurchaseProcess;

    private Button buySkinButton;
    private Button buyMissileButton;
    private Button wearSkinButton;
    private Button wearMissileButton;
    void Awake()
    {
        buySkinButton     = GameObject.Find("btnSkinBuy").GetComponent<Button>();
        buyMissileButton  = GameObject.Find("btnMissileBuy").GetComponent<Button>();
        wearSkinButton    = GameObject.Find("btnSkinWear").GetComponent<Button>();
        wearMissileButton = GameObject.Find("btnMissileWear").GetComponent<Button>();
    }

    public void OnBuySkinButtonClick()
    {
        int price                   = Convert.ToInt32(MarketManagerScript.selectedSkin.VirtualCurrencyPrices["GC"]);
        string skinID               = MarketManagerScript.selectedSkin.ItemId;
       
        PreparePurchaseRequest(price, skinID);                                     
    }
    
    public void PreparePurchaseRequest(int price, string itemID)
    {
        PurchaseItemRequest request = new PurchaseItemRequest();
        request.CatalogVersion  = "RetroRun";
        request.VirtualCurrency = "GC";
        request.Price = price;
        request.ItemId = itemID;        
        PlayFabClientAPI.PurchaseItem(request, InventoryScript.Instance.OnPurchase, OnPlayFabError);

        if (price <= PlayFabGameBridge.userBalance)
        {
            updateMarketItems(request.ItemId);
			OnPurchaseProcess();
            if(itemID.StartsWith("S")){
                buySkinButton.interactable  = false;
                wearSkinButton.interactable = true;
            }
                
            else
            {
                buyMissileButton.interactable = false;
                wearMissileButton.interactable = true;
            }
            
        }
    }

    public void OnBuyMissileButtonClick()
    {
        int price                   = Convert.ToInt32(MarketManagerScript.selectedMissile.VirtualCurrencyPrices["GC"]);
        string missileID            = MarketManagerScript.selectedMissile.ItemId;
        PreparePurchaseRequest(price, missileID);
        
    }

    void OnPlayFabError(PlayFabError error)
    {
        OnError(error.ErrorMessage.ToString());
    }
}
