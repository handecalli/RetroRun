using UnityEngine;
using System.Collections;

public class ParticleColliderScript : MonoBehaviour {

	public ParticleSystem particle;
	public LayerMask groundLayer;

	private bool isFell = false;


	void OnCollisionEnter2D(Collision2D other){

		if(LayerMask.LayerToName(other.gameObject.layer) == "Ground"){
			isFell = true;
		}
	}

	void Update() {
		
		if(isFell){
			particle.Play();
			isFell = false;
		}
	}

}
