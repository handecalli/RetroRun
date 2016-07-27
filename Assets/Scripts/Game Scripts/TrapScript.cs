using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapScript : MonoBehaviour {

	public int number = 3;
	public float hazPosX = 2;
	public float hazPosY = -2;
	public Sprite triggeredTrap;

	private bool isTriggered = false;
	private Vector2 hazardPos;

	private GameObject obj;

	ObjectPoolScript objectPoolScript;
	
	SpriteRenderer sprRend;

	void Awake(){

		sprRend = GetComponent<SpriteRenderer>(); 
		objectPoolScript = GetComponent<ObjectPoolScript> ();
	}

	void Start () {
	
		hazardPos = new Vector2(transform.position.x - hazPosX, transform.position.y - hazPosY);

	}

	void OnTriggerEnter2D(Collider2D other){

		if(!isTriggered && other.tag == "Player") {

			for(int i = 0; i < number; i++) {

				obj = ObjectPoolScript.current.GetPooledObject(0);

				if(obj == null) return;

				obj.transform.position = hazardPos;

				obj.SetActive(true);

			}

			isTriggered = true;
			sprRend.sprite = triggeredTrap;
		}
	}

}
