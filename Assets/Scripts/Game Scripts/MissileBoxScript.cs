using UnityEngine;
using System.Collections;

public class MissileBoxScript : MonoBehaviour {


	private bool isTriggered = false;
	private Collider2D player;

	public delegate void DonateAction(GameObject go);
	public static event DonateAction OnDonate; 


	void OnTriggerEnter2D(Collider2D other){
		
		if(other.tag == "Player") {

			player = other; 
			isTriggered = true;

		}
	}

	void Update() {

		if(isTriggered){
			OnDonate(player.gameObject);
			isTriggered = false;
		}
	}


}
