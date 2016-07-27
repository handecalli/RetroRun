using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour {
    
    public AudioClip goldSound;
    public Text goldtext;

    private int gold = 0;

    void Start()
    {
        goldtext = GameObject.Find("txtGold").GetComponent<Text>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Gold")
        {
            gold++;

            goldtext.text = "Gold: " + gold; 

			PlayFabGameBridge.userBalance += 1;
        }
    }		
}
