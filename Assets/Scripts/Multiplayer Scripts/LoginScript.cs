using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public InputField loginUsername;
    public InputField loginPassword;

    public Button loginButton;

	public InputField registerUsername;
	public InputField registerEmail;
	public InputField registerPassword;
	public InputField registerConfirm;

    public Button registerButton;

    public string levelName = "DontDestroyOnLoad";

    public Animator animToRegister;
    public Animator animToLogin;
    public Animator animToLoginAndRegister;
    public Animator animToAutoLogin;

	public GameObject loginErrorPanel;
	public Text loginErrorText;
	public Text loginErrorTextMessage;
	public GameObject registerErrorPanel;
	public Text registerErrorText;
	public Text registerErrorMessage;
	public GameObject loadingScreen;

	private static string username;
	private string password;
	private bool isLoading;

    void Start()
    {
        string prevUsername = PlayerPrefs.GetString("prevUsername");

        if (prevUsername != "") //if playerPref exists
        {
            Debug.Log("Previous user: " + prevUsername);
            animToAutoLogin.enabled = true;
            animToLoginAndRegister.enabled = true;
            animToAutoLogin.SetBool("isHidden", false);
            animToLoginAndRegister.SetBool("isHidden", true);
			
			isLoading = false;
        }
        else
        {
            Debug.Log("No playerPref");
            animToAutoLogin.SetBool("isHidden", true);
            animToLoginAndRegister.SetBool("isHidden", false);
        }
    }
	
	void Update()
	{
		if (isLoading && Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel("Login");
		}
	}

    public void OnClickLogOut()
    {
        animToAutoLogin.SetBool("isHidden", true);
        animToLoginAndRegister.SetBool("isHidden", false);
    }

    public void OnClickLogin()
    {
		Multiplayer.Instance.Login(loginUsername.text, loginPassword.text);
		username = loginUsername.text;
        password = loginPassword.text;
        PlayerPrefs.SetString("prevUsername", username);
        PlayerPrefs.SetString("prevPassword",password);
	}
	
	public void OnClickRegister()
	{
		Multiplayer.Instance.Register(registerUsername.text, registerPassword.text, registerConfirm.text, registerEmail.text);
	}

    public static string getUsername()
    {
        username = PlayerPrefs.GetString("prevUsername");
		return username;
    }

	public void matchPasswords()
	{
		ColorBlock cb = registerConfirm.colors;

		if(registerConfirm.text != registerPassword.text)
		{
			cb.normalColor = Color.red;
			registerConfirm.colors = cb;
		}
		else
		{
			cb.normalColor = Color.white;
			registerConfirm.colors = cb;
		}
	}

    public void ChangeToLogin()
	{
        animToRegister.SetBool("isHidden", false);
        animToLogin.SetBool("isHidden", true);
    }

    public void ChangeToRegister()
	{
		animToLogin.enabled = true;
		animToRegister.enabled = true;
        animToLogin.SetBool("isHidden", false);
        animToRegister.SetBool("isHidden", true);
    }
	
	public void setLoginErrorText(string errorText, string errorMessage)
	{
		CloseLoading();
		loginErrorPanel.SetActive(true);
		loginErrorText.text = errorText;
		loginErrorTextMessage.text = errorMessage;
	}
	
	public void hideLoginErrorPanel()
	{
		loginErrorPanel.SetActive(false);
	}

	public void setRegisterErrorText(string errorText, string errorMessage)
	{
		CloseLoading();
		registerErrorPanel.SetActive(true);
		registerErrorText.text = errorText;
		registerErrorMessage.text = errorMessage;
	}
	
	public void hideRegisterErrorPanel()
	{
		registerErrorPanel.SetActive(false);
	}

	void OnEnable() 
	{
		Multiplayer.LoggedIn += LetThereBeGame;
		Multiplayer.LoggingIn += ShowLoading;
		Multiplayer.OnLoginError += setLoginErrorText;
		Multiplayer.OnRegisterError += setRegisterErrorText;
	}

	void OnDisable() 
	{
		Multiplayer.LoggedIn -= LetThereBeGame;
		Multiplayer.LoggingIn -= ShowLoading;
		Multiplayer.OnLoginError -= setLoginErrorText;
		Multiplayer.OnRegisterError -= setRegisterErrorText;
	}
	
	public void LetThereBeGame()
	{
        CatalogScript.catalogLoaded = false;
		Application.LoadLevel(levelName);
	}

	public void ShowLoading()
	{
		//loadingScreen = GameObject.Find("pnlLoading").gameObject;
		loadingScreen.SetActive(true);
		isLoading = true;
	}

	public void CloseLoading()
	{
		loadingScreen.SetActive(false);
		isLoading = false;
	}

}
