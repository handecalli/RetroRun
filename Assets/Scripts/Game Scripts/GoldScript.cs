using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldScript : MonoBehaviour{

    public string goldSoundId = "goldSound";

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            AudioEventManager.TriggerPlaySoundID(goldSoundId);
            Destroy(gameObject);
        }
    }
}
