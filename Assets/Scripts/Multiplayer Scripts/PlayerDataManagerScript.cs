using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class PlayerDataManagerScript : GenericSingleton<PlayerDataManagerScript> {
    public delegate void UpdateAction();
    public static event UpdateAction UpdateGrantedItems;

    private bool isRetrived = false;

    public void StartRetriving()
    {
        isRetrived = true;
    }

    protected override void Initialize()
    {
        if (PlayFabData.AuthKey != null) LoadUserData();
        else PlayFabData.LoggedIn += LoadUserData;
    }

    private void LoadUserData(string authKey = null)
    {
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, SetPlayerData, OnPlayFabError);
    }

    void SetPlayerData(GetUserDataResult result)
    {
        if (result.Data.Count == 0)
        {
            PlayFabGameBridge.totalRaces     = 0;
            PlayFabGameBridge.wonRaces       = 0;            
            PlayFabGameBridge.currentSkin    = PlayFabGameBridge.boughtSkins["S00"];
            PlayFabGameBridge.currentMissile = PlayFabGameBridge.boughtMissiles["M00"];

            UpdatePlayerData(PlayFabGameBridge.currentSkin, PlayFabGameBridge.currentMissile);
        }
        else
        {
            PlayFabGameBridge.currentSkin    = PlayFabGameBridge.boughtSkins[result.Data["Skin"].Value];
            PlayFabGameBridge.currentMissile = PlayFabGameBridge.boughtMissiles[result.Data["Missile"].Value];
            PlayFabGameBridge.totalRaces     = int.Parse(result.Data["TotalRaces"].Value);
            PlayFabGameBridge.wonRaces       = int.Parse(result.Data["WonRaces"].Value);
        }        

    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {        
        Debug.Log("Player data successfully retrived");
    }

    void UpdatePlayerData(ItemInstance skin, ItemInstance missile)
    {
        UpdateUserDataRequest request         = new UpdateUserDataRequest();
        Dictionary<string, string> playerData = new Dictionary<string, string>();

        playerData.Add(skin.ItemClass, skin.ItemId);
        playerData.Add(missile.ItemClass, missile.ItemId);
        playerData.Add("TotalRaces", PlayFabGameBridge.totalRaces.ToString());
        playerData.Add("WonRaces", PlayFabGameBridge.wonRaces.ToString());
        
        request.Data = playerData;
        request.Permission = UserDataPermission.Public;

        PlayFabClientAPI.UpdateUserData(request, OnAddDataSuccess, OnPlayFabError);
    }

    private void OnAddDataSuccess(UpdateUserDataResult result)
    {
        UpdateGrantedItems();
        Debug.Log("Granted items are succesfully sent to PlayFab");
    }


    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }
	
}
