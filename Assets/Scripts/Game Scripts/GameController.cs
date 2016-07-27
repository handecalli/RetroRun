using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text fuseText;
    public int missile = 1;

	public Text wonText;
	public Text statsText;
    public Text errorText;
    public Text errorInfoText;

	public GameObject wonPanel;
    public GameObject errorPanel;

    public delegate void DisablePlayer();
    public static event DisablePlayer DisablePlayerController;

    public delegate void PlayerAction();
    public static event PlayerAction JumpClicked;
    public static event PlayerAction FireClicked;

    public static bool IsFinished
    {
        get { return isFinished; }
    }

    private float time;
    private float startTime = 5f;

    private static bool isFinished = false;

    void OnEnable()
    {
        MissileBoxScript.OnDonate += AddFuse;
        PlayerControls.OnMissiled += DecrFuse;
        PlayerControls.OnEnd += OnFinish;
		FinishScript.OnFinishEnd += SetWonText;
        PhotonPlayerScript.ShowErrorOtherPlayerLeftMessage += OtherPlayerLeftMessage;
        PhotonPlayerScript.ShowErrorLoadingGameMessage += ErrorLoadingGameMessage;
        SynchTimeScript.OnShowErrorLoadingMessage += ErrorLoadingGameMessage;
    }

    void OnDisable()
    {
        MissileBoxScript.OnDonate -= AddFuse;
        PlayerControls.OnMissiled -= DecrFuse;
		PlayerControls.OnEnd -= OnFinish;
		FinishScript.OnFinishEnd -= SetWonText;
        PhotonPlayerScript.ShowErrorOtherPlayerLeftMessage -= OtherPlayerLeftMessage;
        PhotonPlayerScript.ShowErrorLoadingGameMessage -= ErrorLoadingGameMessage;
        SynchTimeScript.OnShowErrorLoadingMessage -= ErrorLoadingGameMessage;
    }

    void AddFuse(GameObject go)
    {

        missile++;
        fuseText.text = "Missile: " + missile;
    }

    void DecrFuse(GameObject go)
    {

        missile--;
        fuseText.text = "Missile: " + missile;
    }

	public void SetWonText(int winner, bool isMaster)
	{
		if(isMaster)
		{
			wonPanel.SetActive(true);
			if(winner == 1)
			{
				wonText.text = "Game Over!";
			}
			else if(winner == 2)
			{
				wonText.text = "Game Over!";
			}
			SetStatsText();
		}
		else if(!isMaster)
		{
			wonPanel.SetActive(true);
			if(winner == 2)
			{
				wonText.text = "Game Over!";
			}
			else if(winner == 1)
			{
				wonText.text = "Game Over!";
			}
			SetStatsText();
		}
	}

	public void SetStatsText()
	{
		statsText.text = "You won " + PlayFabGameBridge.wonRaces + " times! \n in " + PlayFabGameBridge.totalRaces + " matches of course. \n You have " + PlayFabGameBridge.userBalance + " gold.";
	}

	public void ReturnToMenu() 
	{
        PhotonNetwork.LeaveRoom();
        CatalogScript.catalogLoaded = false;
        Application.LoadLevel("Menu");
	}

    public void Jump()
    {
        JumpClicked();
    }

    public void Fire()
    {
        FireClicked();
    }

    void OnFinish(GameObject go)
    {
        isFinished = true;
    }

    void OtherPlayerLeftMessage()
    {
        errorText.text = "Sorry";
        errorInfoText.text = "Your opponent left the game.";
        errorPanel.SetActive(true);
    }

    void ErrorLoadingGameMessage()
    {
        errorText.text = "Error";
        errorInfoText.text = "There was an error loading the game. Please try again.";
        errorPanel.SetActive(true);
    }
}
