using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public delegate void StartAction();
    public static event StartAction StartGame;

    public double startTime;
      
    public bool startCountdown = false; 

    private bool startCountdownWhenTimeIsSynced;
    private bool isCountdownFinished = false;
    private bool stopTimer = false;
    private string finishTime = "";

    private const string StartTimeKey = "st";

    public Text timeText;
    public Text startTimeText;

    double elapsedTime;
    double remainingTime;

    void Awake()
    {
        timeText.enabled = false;
    }

    void OnEnable()
    {
        PhotonPlayerScript.OnStartCounter += StartCounter;
        PhotonPlayerScript.StopTimer += StopTimer;
        FinishScript.StopTimer += StopTimer;
    }

    void OnDisable()
    {
        PhotonPlayerScript.OnStartCounter -= StartCounter;
        PhotonPlayerScript.StopTimer -= StopTimer;
        FinishScript.StopTimer -= StopTimer;
    }

    public void StartCounter()
    {
        startCountdown = true;
        this.StartRoundNow();
    }

    private void StartRoundNow()
    {

        if (PhotonNetwork.time < 0.0001f)
        {
            startCountdownWhenTimeIsSynced = true;
            return;
        }
        startCountdownWhenTimeIsSynced = false;

        ExitGames.Client.Photon.Hashtable startTimeProp = new Hashtable();
        startTimeProp[StartTimeKey] = PhotonNetwork.time;
        PhotonNetwork.room.SetCustomProperties(startTimeProp);

    }

    public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
        {
            startTime = (double)propertiesThatChanged[StartTimeKey];
        }
    }

    void Update()
    {
        if (!GameController.IsFinished && !stopTimer)
        {
            elapsedTime = (double)(PhotonNetwork.time - startTime);
        }

        if (startCountdown)
        {
          
            if (startTimeText.enabled == true)
            {       
                remainingTime = 3.00f - elapsedTime;
                startTimeText.text = string.Format("Starts in {0:0}", remainingTime);

                if (remainingTime < 0)
                {
                    Debug.Log("Starting game");
                    startTimeText.enabled = false;
                   // timerStarted = true;
                    timeText.enabled = true;
                    startCountdown = false;
                    StartGame();
                }
            }                               
        }

        if (timeText.enabled)
        {
            timeText.text = string.Format("Time: {0:0.00}", elapsedTime);
        }
    }

    void StopTimer()
    {
        stopTimer = true;
    }

}
