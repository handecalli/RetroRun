using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class LobbyScript : PunBehaviour {

	public Image panelPlayer1;
	public Image panelPlayer2;
    public Image imagePlayer1;
    public Image imagePlayer2;
    public Text namePlayer1;
    public Text namePlayer2;

	public string menu = "Menu";
	public string gameLevel = "MainOn";

    public Button readyButton;

	public GameObject errorPanel;
    public Text errorText;

	public GameObject loadingScreen;
	public Text loadingText;

    public delegate void CheckReadyButton(bool isReady);
    public static event CheckReadyButton On1Ready, On2Ready;

	public bool isPlayer1Ready = false;
	public bool isPlayer2Ready = false;
	private bool isLoading      = true;
    private static bool startCheckingPlayersInLobby = false;

    public static bool StartCheckingPlayersInLobby
    {
        set { startCheckingPlayersInLobby = value; }
    }

    void OnEnable()
    {
        PhotonConnectionScript.Player1StatusReady += Assign1Ready;
        PhotonConnectionScript.Player2StatusReady += Assign2Ready;

        PhotonConnectionScript.Player1Info += SetPanelPlayer1;
        PhotonConnectionScript.Player2Info += SetPanelPlayer2;

        PhotonConnectionScript.ActivateReadyButton += ActivateReadyBtn;

		PhotonConnectionScript.OnLobbyJoin += CloseLoading;
		PhotonConnectionScript.OnConnectionStatus += ConnectionText;
        PhotonConnectionScript.SendErrorMessage += ShowErrorMessage;
    }

	void OnDisable()
	{
		PhotonConnectionScript.Player1StatusReady -= Assign1Ready;
		PhotonConnectionScript.Player2StatusReady -= Assign2Ready;
		
		PhotonConnectionScript.Player1Info -= SetPanelPlayer1;
		PhotonConnectionScript.Player2Info -= SetPanelPlayer2;
		
		PhotonConnectionScript.ActivateReadyButton -= ActivateReadyBtn;
		
		PhotonConnectionScript.OnLobbyJoin -= CloseLoading;
		PhotonConnectionScript.OnConnectionStatus -= ConnectionText;
        PhotonConnectionScript.SendErrorMessage -= ShowErrorMessage;
	}

	void Update()
	{
		if(isPlayer1Ready)
		{
			panelPlayer1.color = Color.green;
            isPlayer1Ready = true;
		}
		else
		{
			panelPlayer1.color = Color.red;
		}

		if(isPlayer2Ready)
		{
			panelPlayer2.color = Color.green;
            isPlayer2Ready = true;
		}
		else
		{
			panelPlayer2.color = Color.red;
		}

        if (isPlayer1Ready && isPlayer2Ready)
        {
            HideRoomFromNewPlayers(); 
            CatalogScript.catalogLoaded = false;

            //Random level generator. 
            //Level list başka bir yerde generate edilecek ve ona göre random level'da oyun başlayacak.
            //string[] levelList = new string[5];
            //int levelIndex = Random.Range(0, 5);

            //string currentLevel = levelList[levelIndex];

            Application.LoadLevel(gameLevel);
		}

		if (isLoading && Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel("Login");
		}

        if(startCheckingPlayersInLobby)
        {
            CheckPlayerCountInLobby();
        }
	}

    void SetPanelPlayer1(string skinID1, string username1)
    {
        imagePlayer1.sprite = Resources.Load<Sprite>("SkinImages/" + skinID1) as Sprite;
        imagePlayer1.color = new Color(1f, 1f, 1f, 1f);
        namePlayer1.text = username1;
    }

    void SetPanelPlayer2(string skinID2, string username2)
    {
        imagePlayer2.sprite = Resources.Load<Sprite>("SkinImages/" + skinID2) as Sprite; 
        imagePlayer2.color = new Color(1f, 1f, 1f, 1f);
        namePlayer2.text = username2;
    }

    void DeletePanelPlayer2()
    {
        namePlayer2.text = "Player2";
        imagePlayer2.sprite = null;
        imagePlayer2.color = new Color(0f, 0f, 0f, 0f);
    }

    void Assign1Ready(bool is1Ready)
    {
        isPlayer1Ready = is1Ready;
    }

    void Assign2Ready(bool is2Ready)
    {
        isPlayer2Ready = is2Ready;
    }

	public void PlayerReady()
	{
        if (PhotonNetwork.isMasterClient)
        {
            if (!isPlayer1Ready)
            {
                isPlayer1Ready = true;
                Debug.Log("Player1Ready = " + isPlayer1Ready);
                On1Ready(isPlayer1Ready);
            }
            else
            {
                isPlayer1Ready = false;
                Debug.Log("Player1Ready = " + isPlayer1Ready);
                On1Ready(isPlayer1Ready);
            }
        }
        else
        {
            if (!isPlayer2Ready)
            {
                isPlayer2Ready = true;
                Debug.Log("Player2Ready = " + isPlayer2Ready);
                On2Ready(isPlayer2Ready);
            }
            else
            {
                isPlayer2Ready = false;
                Debug.Log("Player2Ready = " + isPlayer2Ready);
                On2Ready(isPlayer2Ready);
            }
        }
    }

	public void GoToMenu()
	{
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Player1Ready = " + isPlayer1Ready);
            On1Ready(isPlayer1Ready);
            photonView.RPC("RPCMasterLeft", PhotonTargets.Others);
        }
        else
        {
            Debug.Log("Player2Ready = " + isPlayer2Ready);
            On2Ready(isPlayer2Ready);
            photonView.RPC("RPCOtherClientLeft", PhotonTargets.Others);
        }

        PhotonNetwork.LeaveRoom();
        CatalogScript.catalogLoaded = false;
        Application.LoadLevel(menu);
	}

    void ActivateReadyBtn()
    {
        readyButton.interactable = true;
    }

    void DeactivateReadyBtn()
    {
        readyButton.interactable = false;
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

	public void ConnectionText(string connectionStatus)
	{
		loadingText.text = connectionStatus;
	}

    [PunRPC]
    void RPCMasterLeft()
    {
        MasterLeftAction();
    }

    [PunRPC]
    void RPCOtherClientLeft()
    {
        OtherClientLeftAction();
    }

    void MasterLeftAction()
    {
        DeactivateReadyBtn();
        SetPanelPlayer1(PlayFabGameBridge.currentSkin.ItemId, LoginScript.getUsername());
        DeletePanelPlayer2();
    }

    void OtherClientLeftAction()
    {
        DeactivateReadyBtn();
        namePlayer2.text = "Player2";
        imagePlayer2.sprite = null;
        imagePlayer2.color = new Color(0f, 0f, 0f, 0f);
    }

    void OnMasterClientSwitched()
    {
        Debug.Log("New master client: " + PhotonNetwork.masterClient.ToString() + " == " + PhotonNetwork.player.ToString());
    }

    void CheckPlayerCountInLobby()
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.room.playerCount < 2)
            {
                isPlayer1Ready = false;
                isPlayer2Ready = false;
                if (LoginScript.getUsername() == namePlayer1.text)
                {
                    MasterLeftAction();
                }
                else
                {
                    OtherClientLeftAction();
                }
                startCheckingPlayersInLobby = false;
            }
        }
    }

    void HideRoomFromNewPlayers()
    {
        PhotonNetwork.room.open = false;
    }

    void ShowErrorMessage(string message)
    {
        errorText.text = message;
        errorPanel.SetActive(true);
        CatalogScript.catalogLoaded = false;
        Application.LoadLevel(menu);
    }

}
