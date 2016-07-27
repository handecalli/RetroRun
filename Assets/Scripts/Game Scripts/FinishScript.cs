using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class FinishScript : MonoBehaviour {

    public delegate void TimerAction();
    public static event TimerAction StopTimer;

	public delegate void FinishAction(GameObject go);
	public static event FinishAction OnFinished;    
	public static event FinishAction OnFinishWon;
	
	public delegate void FinisherAction(int winner, bool isMaster);
	public static event FinisherAction OnFinishEnd;    

	public List<GameObject> finishers;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player" && other.isTrigger) {					

			finishers.Add(other.gameObject);
			PlayFabGameBridge.totalRaces++;
            StopTimer();

            if (finishers[0] == other.gameObject)
			{
				PlayFabGameBridge.wonRaces++;
				Debug.Log(PlayFabGameBridge.wonRaces);
				OnFinishWon(other.gameObject);
				if(finishers.Count >= 2)
				{
					if(PhotonNetwork.isMasterClient)
					{
						OnFinishEnd(1, true);
					}
					else
					{
						OnFinishEnd(2, false);
					}
				}
			}

            OnFinished(other.gameObject);
			if(finishers.Count >= 2)
			{
				if(PhotonNetwork.isMasterClient)
				{
					OnFinishEnd(2, true);
				}
				else
				{
					OnFinishEnd(1, false);
				}
			}
		}
	}
}
