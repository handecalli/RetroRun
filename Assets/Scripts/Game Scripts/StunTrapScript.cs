using UnityEngine;
using System.Collections;

public class StunTrapScript : MonoBehaviour {
	
	public Sprite triggeredTrap;

	private bool isTriggered = false;
	
	SpriteRenderer sprRend;
	
	public delegate void StunAction(GameObject go);
	public static event StunAction OnStunned; 


	void Awake(){
		
		sprRend = GetComponent<SpriteRenderer>();
	}


	
	void OnTriggerEnter2D(Collider2D other){
		
		if(!isTriggered && other.tag == "Player") {

			OnStunned(other.gameObject);

			isTriggered = true;
			sprRend.sprite = triggeredTrap;
		}
	}


}
