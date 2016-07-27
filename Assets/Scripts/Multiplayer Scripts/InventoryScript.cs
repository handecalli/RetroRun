using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab;
public class InventoryScript : GenericSingleton<InventoryScript>
{    
    public static List<ItemInstance> inventory;
    public static bool InventoryLoaded = false;

    public delegate void UpdateAction();
    public static event UpdateAction UpdateGoldAmmount;	
	public static event UpdateAction PurchaseDone;
        
    protected override void Initialize()
    {
        PlayFabGameBridge.boughtSkins    = new Dictionary<string, ItemInstance>();
        PlayFabGameBridge.boughtMissiles = new Dictionary<string, ItemInstance>();
        
    }

    public void CreateInventory()
    {
        if 
            (PlayFabData.AuthKey != null) UpdateInventory();
        else
            PlayFabData.LoggedIn += UpdateInventory;        
    }

    public void UpdateInventory(string authKey = null)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnUpdateUserInventory, OnPlayFabError);        
    }
    

    public void OnUpdateUserInventory(GetUserInventoryResult result) 
    {      
        //Player inventory result after reques. It could be empty if it is a new player.
        inventory = result.Inventory;

        //If inventory is empty grant default items
        if (inventory.Count == 0)
        {
            Debug.Log("GrantItemAtInventory");
            GrantItems();
        }

        //for every skin and missile bought update market
        for (int i = 0; i < inventory.Count; i++)
        {
            ItemInstance itemInstance = inventory[i];

            if (itemInstance.ItemClass == "Skin" && !PlayFabGameBridge.boughtSkins.ContainsKey(itemInstance.ItemId))
            {
                PlayFabGameBridge.boughtSkins.Add(itemInstance.ItemId, itemInstance);
            }
            else if (itemInstance.ItemClass == "Missile" && !PlayFabGameBridge.boughtMissiles.ContainsKey(itemInstance.ItemId))
            {
                PlayFabGameBridge.boughtMissiles.Add(itemInstance.ItemId, itemInstance);
            }
        }         

        //Send event (if needed) that loading inventory process is done
        if (inventory.Count == (PlayFabGameBridge.boughtSkins.Count + PlayFabGameBridge.boughtMissiles.Count))
        {
            InventoryLoaded = true;	
			PurchaseDone();
        }

        //Go to the next execution in InitializationNext function
        if (InventoryLoaded)
        {
            InitializationEventManager.OnNextInitialization();
        }  

    }
    
    void GrantItems()
    {
        string defaultSkinID    = PlayFabGameBridge.defaultSkin.skinID;
        string defaultMissileID = PlayFabGameBridge.defaultMissile.missileID;
        int defaultSkinPrice    = (int)PlayFabGameBridge.defaultSkin.price;
        int defaultMissilePrice = (int)PlayFabGameBridge.defaultMissile.price;

        PurchaseItemRequest skinRequest = new PurchaseItemRequest();
        skinRequest.CatalogVersion = "RetroRun";
        skinRequest.VirtualCurrency = "GC";
        skinRequest.Price = defaultSkinPrice;
        skinRequest.ItemId = defaultSkinID;

        PlayFabClientAPI.PurchaseItem(skinRequest, InventoryScript.Instance.OnPurchase, OnPlayFabError);

        PurchaseItemRequest missileRequest = new PurchaseItemRequest();
        missileRequest.CatalogVersion = "RetroRun";
        missileRequest.VirtualCurrency = "GC";
        missileRequest.Price = defaultMissilePrice;
        missileRequest.ItemId = defaultMissileID;

        PlayFabClientAPI.PurchaseItem(missileRequest, InventoryScript.Instance.OnPurchase, OnPlayFabError);
        
    }
        
    public void OnPurchase(PurchaseItemResult result)
    {        
        PlayFabGameBridge.userBalance -= (int) result.Items[0].UnitPrice;
        UpdateGoldAmmount();
        InventoryScript.Instance.UpdateInventory();        
    }
    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }

}
