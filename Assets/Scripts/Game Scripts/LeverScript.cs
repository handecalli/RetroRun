using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverScript : MonoBehaviour {

	
	public List<ObjectTypes> ObjectsList = new List<ObjectTypes>();
	
	private bool isTriggered = false;


	[System.Serializable]
	public struct ObjectTypes {  
		
		public GameObject lockerObject;
		public bool enableObject;
	}

	void OnTriggerEnter2D(Collider2D other){
		
		if(!isTriggered && (other.tag == "Player" || other.tag == "Hazard")) 
		{

			isTriggered = true;

			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;

			for(int i = 0; i < ObjectsList.Count; i++)
			{
				if(ObjectsList[i].enableObject)
				{
					ObjectsList[i].lockerObject.SetActive(true);
				}
				else
				{
					ObjectsList[i].lockerObject.SetActive(false);
				}
			}
		}
	}

}
