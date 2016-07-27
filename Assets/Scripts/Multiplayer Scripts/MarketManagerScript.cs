using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using PlayFab.ClientModels;
using PlayFab;
public class MarketManagerScript : MonoBehaviour {

	public Text playerNameText;
    public Text goldAmmountText;
    public Text purchaseErrorText;

	public GameObject skinPanel;
	public GameObject missilePanel;
	public GameObject otherPanel;
	public GameObject errorPanel;
	public GameObject loadingScreen;
	
	public Scrollbar scrollbarSkin;
	public Scrollbar scrollbarMissile;

	public string menuScene = "Menu";

    public static CatalogItem selectedSkin;
    public static CatalogItem selectedMissile;
	
	private bool isLoading;

    int ExecutionOrder = 0;

	void Start() {
		
		isLoading = false;
        PlayFabClientAPI.GetUserCombinedInfo(new GetUserCombinedInfoRequest(), OnGetUserCurrency, OnPlayFabError);        
		playerNameText.text = LoginScript.getUsername();
        InitializationNext();

	}

    private void OnGetUserCurrency(GetUserCombinedInfoResult result)
    {
        PlayFabGameBridge.userBalance = result.VirtualCurrency["GC"];
        goldAmmountText.text =  "Gold\n " + PlayFabGameBridge.userBalance.ToString("0000");
       
    } 

	public void ShowSkinPanel() {
		
		missilePanel.SetActive(false);
		otherPanel  .SetActive(false);
		skinPanel   .SetActive(true );
		scrollbarSkin.value = 0;
	}

	public void ShowMissilePanel() {
		
		skinPanel   .SetActive(false);
		otherPanel  .SetActive(false);
		missilePanel.SetActive(true );
		scrollbarMissile.value = 0;
	}

	public void ShowOtherPanel() {
		
		skinPanel   .SetActive(false);
		missilePanel.SetActive(false);
		otherPanel  .SetActive(true );
	}
    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }

    void OnEnable() 
    {
       
        InitializationEventManager.OnNextInitialization += InitializationNext;
        InventoryScript.UpdateGoldAmmount += UpdateGoldAmmount;
        PurchaseScript.OnPurchaseProcess += ShowLoading;
        PurchaseScript.OnError += GiveError;   
		InventoryScript.PurchaseDone += CloseLoading;
    }

    void OnDisable()
    {
        InitializationEventManager.OnNextInitialization -= InitializationNext;
        InventoryScript.UpdateGoldAmmount -= UpdateGoldAmmount;
        PurchaseScript.OnPurchaseProcess -= ShowLoading;
        PurchaseScript.OnError -= GiveError;
        InventoryScript.PurchaseDone -= CloseLoading;
      
    }

	public void CloseLoading ()
	{
		loadingScreen.SetActive(false);
		isLoading = false;
	}
	
	public void ShowLoading ()
	{
		loadingScreen.SetActive(true);
		isLoading = true;
	}

    void InitializationNext()
    {
        switch (ExecutionOrder)
        {
            case 0:
                InventoryScript.Instance.CreateInventory();
                break;
            case 1:
                CatalogScript.Instance.CreateCatalog();
                break; 
            case 2:
                PlayerDataManagerScript.Instance.StartRetriving();
                break;
            default:
                break;
        }

        ExecutionOrder++;
    }

    void GiveError(string error)
    {
        errorPanel.SetActive(true);
        purchaseErrorText.text = error;
    }


    void UpdateGoldAmmount()
    {
        goldAmmountText.text = "Gold\n " + PlayFabGameBridge.userBalance.ToString("0000");
    }

    public void CloseErrorPanel()
    {
        errorPanel.SetActive(false);
    }

}
