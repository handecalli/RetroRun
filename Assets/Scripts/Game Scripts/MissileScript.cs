using UnityEngine;
using System.Collections;

public class MissileScript : MonoBehaviour {

	public delegate void StunnedAction(GameObject go);
	public static event StunnedAction OnStun; 
	
	void OnCollisionEnter2D(Collision2D other){
		
		if(other.gameObject.tag == "Player") {

			OnStun(other.gameObject);
		}
	}

}
