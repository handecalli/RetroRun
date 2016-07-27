using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using PlayFab.Serialization.JsonFx;
public class CatalogScript : GenericSingleton<CatalogScript>
{
    public static List<CatalogItem> items;
    public delegate void MarketAction();
    public static event MarketAction InitMarketItems;

    public static bool catalogLoaded = false;
    protected override void Initialize()
    {
        PlayFabGameBridge.skins = new Dictionary<string, Skin>();
        PlayFabGameBridge.missiles = new Dictionary<string, Missile>();
    }

    private void LoadMarketMenu(string authKey = null)
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = PlayFabData.CatalogVersion;
        PlayFabClientAPI.GetCatalogItems(request, ConstructCatalog, OnPlayFabError);
    }

    public void CreateCatalog()
    {
        if (PlayFabData.AuthKey != null)
            LoadMarketMenu();
        else
            PlayFabData.LoggedIn += LoadMarketMenu;
    }

    private void ConstructCatalog(GetCatalogItemsResult result) 
    {
        items = result.Catalog;
        if (!PlayFabGameBridge.catalogConstructed)
        {
            //adding default skin and missile
            PlayFabGameBridge.skins.Add(PlayFabGameBridge.defaultSkin.skinID, PlayFabGameBridge.defaultSkin);
            PlayFabGameBridge.missiles.Add(PlayFabGameBridge.defaultMissile.missileID, PlayFabGameBridge.defaultMissile);

            //Now, inserting items to dictionaries
            for (int i = 0; i < items.Count; i++)
            {
                Dictionary<string, uint> priceList = items[i].VirtualCurrencyPrices;

                if (items[i].ItemClass.StartsWith("Skin") && !PlayFabGameBridge.skins.ContainsKey(items[i].ItemId))
                {
                    //Creating a new skin object
                    Skin newSkin = new Skin
                    {
                        skinID = items[i].ItemId,
                        skinName = items[i].DisplayName,
                        price = priceList["GC"]
                    };


                    PlayFabGameBridge.skins.Add(items[i].ItemId, newSkin);
                }

                if (items[i].ItemClass.StartsWith("Missile") && !PlayFabGameBridge.missiles.ContainsKey(items[i].ItemId))
                {
                    Missile newMissile = new Missile
                    {
                        missileID = items[i].ItemId,
                        missileName = items[i].DisplayName,
                        price = priceList["GC"]
                    };
                    PlayFabGameBridge.missiles.Add(items[i].ItemId, newMissile);
                }

            }
            
            catalogLoaded = true;
            PlayFabGameBridge.catalogConstructed = true;
        }
        TriggerInitMarket(); 
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }

    void TriggerInitMarket() {
        if (InitMarketItems != null)
        {
            InitMarketItems();
        }
    }
}
