using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon;

public class SynchTimeScript : PunBehaviour
{
    //for testing purposes
    //debug allbuffered switch
    public static bool allBuffered = false;

    public delegate void InstantiationAction();
    public static event InstantiationAction InstantiatePlayers;

    public delegate void OnErrorEvent();
    public static event OnErrorEvent OnShowErrorLoadingMessage;

    public float waitForOtherClientInterval = 2f;
    public float maxWaitForOtherClient = 10f;

    private bool isClientSearching = false;
    private float clientSearchStartTime = 0.0f;
    private float timeout = 0.0f;

    //This function starts as a player loads the game
    void Start()
    {
        //There are two players in the game.
        //One of them is a master client and the other one is just a client.
        //In this way we can distinguish between players.
        //One of the clients (in this case master client) starts sending RPC until he receives and acknowledgement from other player
        if (!PhotonNetwork.isMasterClient)
        {
            photonView.RPC("OnClientConnected", PhotonTargets.OthersBuffered);
            isClientSearching = true;
            timeout = Time.time;
            clientSearchStartTime = Time.time;
        }
        else // debug purposes. remove else at the end of project
        {
            if (allBuffered)
            {
                InstantiatePlayers();
            }
        }

    }

    //Update is run every frame in the scene.
    void Update() {

        //Each clint searches for the other to start the countdown for a specific period of time. 
        //In every clientSearchStartTime (5 seconds in this case) client sends RPC and waits for the response
        //There is also a timeout for players so that if other player is not coming they can be informed
        if (isClientSearching) 
        {
            if (Time.time - clientSearchStartTime > waitForOtherClientInterval) 
            {
                //photonView.RPC(...) functions are RPC sending functions over photon network
                photonView.RPC("OnClientConnected", PhotonTargets.OthersBuffered);
                clientSearchStartTime = Time.time;
            }
            if (Time.time - timeout > maxWaitForOtherClient)
            {
                isClientSearching = false;
                OnShowErrorLoadingMessage();
            }
        }
    
    }

    //[PunRPC] functions are RPC receiving function

    //When non-master client connects, master client receieves this function and starts the game
    [PunRPC]
    void OnClientConnected() {

        photonView.RPC("OnMasterAcknowledges", PhotonTargets.OthersBuffered);
        InstantiatePlayers();

    }

    //When master receives RPC from non-master, he sends an acknowledgement and starts the game
    [PunRPC]
    void OnMasterAcknowledges() {

        isClientSearching = false;
        InstantiatePlayers();
    }

}
