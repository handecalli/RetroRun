using UnityEngine;
using System.Collections;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PhotonPlayerScript : PunBehaviour {

	public Image minimapChar;

	public Transform startZone;

    public delegate void StartAction();
    public static event StartAction OnStartCounter;
    public static event StartAction InitMinimap;

    public delegate void SendError();
    public static event SendError ShowErrorOtherPlayerLeftMessage;
    public static event SendError ShowErrorLoadingGameMessage;

    public delegate void StopAction();
    public static event StopAction StopTimer;
    public static event StopAction StopMinimap;

    private PlayerControls controller;
    private PhotonTargets bufferMode;
	private GameObject selectedSkin;
    private GameObject playerPrefab;
    private GameObject otherPlayer;
	private Sprite selectedMissile;

    private bool controllerEnabled = false;
    private bool isPlayer1Created = false;
    private bool isPlayer2Created = false;

    void Awake()
    {
        bufferMode = SynchTimeScript.allBuffered ? PhotonTargets.AllBuffered : PhotonTargets.OthersBuffered;
    }

    void Update()
    {
        if (isPlayer1Created && isPlayer2Created)
        {
            CheckPlayerCountInRoom();
            InitMinimap();
            isPlayer1Created = false;
            isPlayer2Created = false;
        }
    }

    private void GetSelectedItems()
	{
		Debug.Log("Getting player items...");
		Debug.Log(PlayFabGameBridge.currentSkin.ItemId);
		playerPrefab = Resources.Load(PlayFabGameBridge.currentSkin.ItemId) as GameObject;
		Debug.Log(PlayFabGameBridge.currentMissile.ItemId);
		selectedMissile = Resources.Load<Sprite>(PlayFabGameBridge.currentMissile.ItemId) as Sprite;
	}

    void RPCToInstantiateOther()
    {
        photonView.RPC("InstantiateMe", bufferMode);
    }

    [PunRPC]
    void InstantiateMe()
    {
        GetSelectedItems();
        selectedSkin = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(startZone.position.x, startZone.position.y, 0f), Quaternion.identity, 0);
        selectedSkin.GetComponent<PlayerControls>().missileSkin = selectedMissile;
        CameraControllerOnline cameraController = selectedSkin.GetComponent<CameraControllerOnline>();
        cameraController.enabled = true;
        selectedSkin.GetComponent<Rigidbody2D>().isKinematic = false;
        isPlayer1Created = true;

        photonView.RPC("RPCOnStartCounter", bufferMode);

    }

    [PunRPC]
    void RPCOnStartCounter()
    {
        OnStartCounter();
        isPlayer2Created = true;
    }

    void OnEnable()
    {
        Timer.StartGame += SendEnableControllerRPC;
        SynchTimeScript.InstantiatePlayers += RPCToInstantiateOther;
        GameController.DisablePlayerController += DisablePlayerController;
    }

    void OnDisable()
    {
        Timer.StartGame -= SendEnableControllerRPC;
        SynchTimeScript.InstantiatePlayers -= RPCToInstantiateOther;
    }

    void SendEnableControllerRPC()
    {
        photonView.RPC("RPCEnableController", bufferMode);
    }

    [PunRPC]
    void RPCEnableController()
    {
		controller = selectedSkin.GetComponent<PlayerControls>();
        controller.enabled = true;
        controllerEnabled = true;
    }

    void CheckPlayerCountInRoom()
    {
        if (!GameController.IsFinished)
        {
            if (PhotonNetwork.room.playerCount < 2)
            {
                DisablePlayerController();
                StopMinimap();
                StopTimer();
            }
        }
    }

    void DisablePlayerController()
    {
        if (controllerEnabled)
        {
            controller.enabled = false;
            StopMinimap();
            StopTimer();
            ShowErrorOtherPlayerLeftMessage();
        }
        else
        {
            ShowErrorLoadingGameMessage();
        }
    }
}
