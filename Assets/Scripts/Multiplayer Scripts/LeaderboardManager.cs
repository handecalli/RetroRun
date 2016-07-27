using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {
	
	public static List<string> usernames;
	public uint orderNo = 0;
	public GameObject ListItem;
	public Text txtNo;
	public Text txtName;
	public Text txtScore;
	public GameObject BoardContentPanel;

	private Dictionary<string, uint> LeaderboardHighScores = new Dictionary<string, uint>();

	
	void Start () {
		if (PlayFabData.AuthKey != null) InitLeaderboard();
		else PlayFabData.LoggedIn += InitLeaderboard;
	}

	public void InitLeaderboard(string authKey = null) 
	{
		PlayFab.ClientModels.GetLeaderboardRequest lbRequest = new PlayFab.ClientModels.GetLeaderboardRequest();
		lbRequest.MaxResultsCount = 10;
		lbRequest.StatisticName = "WonRaces";
		PlayFabClientAPI.GetLeaderboard(lbRequest, LoadLeaderboard, OnPlayFabError);
	}

	public void LoadLeaderboard (PlayFab.ClientModels.GetLeaderboardResult lbResult)
	{
		LeaderboardHighScores.Clear();

		foreach (PlayFab.ClientModels.PlayerLeaderboardEntry entry in lbResult.Leaderboard)
		{
			if(entry.DisplayName != null)
			{
				LeaderboardHighScores.Add (entry.DisplayName, (uint)entry.StatValue);				
			}
			else
			{
				LeaderboardHighScores.Add (entry.PlayFabId, (uint)entry.StatValue);
			}
		}
		Initialize();
	}

	public void Initialize()
	{
		foreach(KeyValuePair<string, uint> PlayerScore in LeaderboardHighScores)
		{
			orderNo++;

			txtNo.text = orderNo.ToString();
			txtName.text = PlayerScore.Key.ToString();
			txtScore.text = PlayerScore.Value.ToString();
			//Debug.Log(orderNo.ToString() + " - " + PlayerScore.Key.ToString() + " - " + PlayerScore.Value.ToString());
			
			GameObject item = Instantiate(ListItem);
			item.transform.SetParent(BoardContentPanel.transform);
		}
	}


	void OnPlayFabError(PlayFabError error)
	{
		Debug.Log ("Got an error: " + error.ErrorMessage);
	}

}
