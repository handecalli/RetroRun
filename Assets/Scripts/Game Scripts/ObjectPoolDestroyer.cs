using UnityEngine;
using System.Collections;

public class ObjectPoolDestroyer : MonoBehaviour {

	public float destroyTime = 10f;

	void OnEnable() {

		Invoke("Destroy", destroyTime);
	}
	
	void Destroy() {
		
		gameObject.SetActive(false);
	}

	void OnDisable() {

		CancelInvoke();
	}

}
