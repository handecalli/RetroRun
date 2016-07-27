using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
public class ItemWearScript : MonoBehaviour {

	
	private Button wearSkinButton;
	private Button wearMissileButton;

	void Awake()
	{
		wearSkinButton    = GameObject.Find("btnSkinWear").GetComponent<Button>();
		wearMissileButton = GameObject.Find("btnMissileWear").GetComponent<Button>();
	}

	public void WearSkin()
	{
        PlayFabGameBridge.currentSkin = PlayFabGameBridge.boughtSkins[MarketManagerScript.selectedSkin.ItemId];
        UpdatePlayerData(PlayFabGameBridge.currentSkin);
		
        wearSkinButton.interactable = false;
		ColorBlock cb = wearSkinButton.colors;
		cb.disabledColor = Color.green;
		wearSkinButton.colors = cb;

	}

    public void WearMissile()
    {
        PlayFabGameBridge.currentMissile = PlayFabGameBridge.boughtMissiles[MarketManagerScript.selectedMissile.ItemId];
		UpdatePlayerData(PlayFabGameBridge.currentMissile);
		
        wearMissileButton.interactable = false;
		ColorBlock cb = wearMissileButton.colors;
		cb.disabledColor = Color.green;
		wearMissileButton.colors = cb;
	}
	
    void UpdatePlayerData(ItemInstance item)
    {
        UpdateUserDataRequest request = new UpdateUserDataRequest();
        Dictionary<string, string> playerData = new Dictionary<string,string>();
        playerData.Add(item.ItemClass, item.ItemId);
        request.Data = playerData;
        request.Permission = UserDataPermission.Public;

        PlayFabClientAPI.UpdateUserData(request, OnAddDataSuccess, OnAddDataError);
    }

    void OnAddDataSuccess(UpdateUserDataResult result) 
    {
        Debug.Log("Player data succesfully updated");
    }

    void OnAddDataError(PlayFabError error)
    {
        Debug.Log("Add data error: " + error.Error + " " + error.ErrorMessage);
    }
}
