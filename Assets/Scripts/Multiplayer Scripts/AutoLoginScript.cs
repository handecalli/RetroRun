using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoLoginScript : MonoBehaviour {

    public Text loginAsUser;

    private string prevUsername;
    private string prevPassword;

    void Start()
    { 
        prevUsername = PlayerPrefs.GetString("prevUsername");
        prevPassword = PlayerPrefs.GetString("prevPassword");   
        loginAsUser.text = "Login As " + prevUsername;
    }

    public void LoginAs()
    {
        Multiplayer.Instance.Login(prevUsername, prevPassword);
    }

    public void Logout()
    {
        Debug.Log(PlayerPrefs.GetString("prevUsername") + " deleted.");

        PlayerPrefs.DeleteKey("prevUsername");
        PlayerPrefs.DeleteKey("prevPassword");
    }
}
