using UnityEngine;
using System.Collections;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PhotonConnectionScript : PunBehaviour {

    public delegate void PlayerStatus(bool isPlayerReady);
    public static event PlayerStatus Player1StatusReady, Player2StatusReady;

	public delegate void ActivateButton();
	public static event ActivateButton ActivateReadyButton;
	public static event ActivateButton OnLobbyJoin;

    public delegate void PlayerInfo(string skinID, string username);
    public static event PlayerInfo Player1Info, Player2Info;

	public delegate void LoadStatus(string connectionMessage);
	public static event LoadStatus OnConnectionStatus;

    public delegate void SendError(string message);
    public static event SendError SendErrorMessage;

	private static string skinID1, skinID2 ;
	private string username1, username2;
    private bool prevStatus1 = false;
    private bool prevStatus2 = false;
	private string connectionStatus;
    private string errorMessage;


	void Start()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
            if (Multiplayer.connectedToPlayFab)
            {
                GetPhotonAuthenticationToken();
            } 
        }
        else
        {
            JoinLobby();
        }
        connectionStatus = PhotonNetwork.connectionStateDetailed.ToString();
    }

	void Update()
	{
		if(connectionStatus != PhotonNetwork.connectionStateDetailed.ToString())
		{
			OnConnectionStatus(PhotonNetwork.connectionStateDetailed.ToString());
			connectionStatus = PhotonNetwork.connectionStateDetailed.ToString();
		}
	}

    void GetPhotonAuthenticationToken()
    {

        GetPhotonAuthenticationTokenRequest req = new GetPhotonAuthenticationTokenRequest();
        req.PhotonApplicationId = "f5b794c3-caa6-4d84-9b66-07726628b4de";

        PlayFabClientAPI.GetPhotonAuthenticationToken(req, OnPhotonAuthenticationSuccess, OnPlayfabError);

    }

    void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
    {

        Debug.Log("Photon Login Success");
        ConnectToMasterServer(Multiplayer.playfabId, result.PhotonCustomAuthenticationToken);

    }

    void OnPlayfabError(PlayFabError error)
    {

        Debug.Log(error.Error.ToString());
        Debug.Log(error.ErrorMessage);
        Debug.Log(error.ErrorDetails);

    }

    private void ConnectToMasterServer(string id, string ticket)
    {
        if (PhotonNetwork.AuthValues != null)
        {

        }
        else
        {

            PhotonNetwork.AuthValues = new AuthenticationValues()
            {

                AuthType = CustomAuthenticationType.Custom

            };

            PhotonNetwork.AuthValues.AddAuthParameter(id, ticket);

        }

        PhotonNetwork.ConnectUsingSettings("1.0");

        // LoadLevelEvent();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected to Master");

        JoinLobby();
    }

    public void JoinLobby()
    {
        Debug.Log("JoinLobby");
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        JoinRandomRoom();
    }

    void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMessage)
    {
        Debug.Log("New room has been created");

        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
		OnLobbyJoin();
        if (!PhotonNetwork.isMasterClient)
        {
            ActivateReadyButton();
            skinID2 = PlayFabGameBridge.currentSkin.ItemId;
            username2 = LoginScript.getUsername();

            Player2Info(skinID2, username2);

            Debug.Log("Player2 sent his skinID and username");
            photonView.RPC("RPCOtherClientSkin", PhotonTargets.Others, skinID2, username2);
        }
        else
        {
            skinID1 =  PlayFabGameBridge.currentSkin.ItemId;
            username1 = LoginScript.getUsername();

            Player1Info(skinID1, username1);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        ActivateReadyButton();
        LobbyScript.StartCheckingPlayersInLobby = true;
        Debug.Log("Player1 sent his skinID " + skinID1 + "and username " + username1);
        if (skinID1 == null || username1 == null)
        {
            skinID1 = PlayFabGameBridge.currentSkin.ItemId;
            username1 = LoginScript.getUsername();
        }

        photonView.RPC("RPCMasterClientSkin", PhotonTargets.Others, skinID1, username1);
    }

    void OnEnable()
    {
        LobbyScript.On1Ready += MasterClientReady;
        LobbyScript.On2Ready += OtherClientReady;
    }

    void OnDisable()
    {
        LobbyScript.On1Ready -= MasterClientReady;
        LobbyScript.On2Ready -= OtherClientReady;
    }

    void MasterClientReady(bool isReady)
    {
        if (prevStatus1 != isReady)
        {
            photonView.RPC("RPCMasterClientStatus", PhotonTargets.Others, isReady);
            prevStatus1 = isReady;
        }
    }

    void OtherClientReady(bool isReady)
    {
        if (prevStatus2 != isReady)
        {
            photonView.RPC("RPCOtherClientStatus", PhotonTargets.Others, isReady);
            prevStatus2 = isReady;
        }
    }

    [PunRPC]
    void RPCMasterClientStatus(bool isReady)
    {
        Player1StatusReady(isReady);
    }

    [PunRPC]
    void RPCOtherClientStatus(bool isReady)
    {
        Player2StatusReady(isReady);
    }

    [PunRPC]
    void RPCMasterClientSkin(string skinID, string username)
    {
        Player1Info(skinID, username);
    }

    [PunRPC]
    void RPCOtherClientSkin(string skinID, string username)
    {
        Player2Info(skinID, username);
    }

	public static string GetSkinID1()
	{
		return skinID1;
	}

	public static string GetSkinID2()
	{
		return skinID2;
	}

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        errorMessage = "Failed to Connect. Please try again.";
        SendErrorMessage(errorMessage);
    }

    void OnDisconnectedFromPhoton()
    {
        errorMessage = "Disconnected. Please try again.";
        SendErrorMessage(errorMessage);
    }
}
