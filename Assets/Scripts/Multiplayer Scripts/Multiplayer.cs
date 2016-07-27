using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;

public class Multiplayer : GenericSingleton<Multiplayer>
    {
    public GameObject hostPlayer;
    public GameObject visitingPlayer;   

	public delegate void AuthorizationFinishAction();
	public static event AuthorizationFinishAction LoggingIn;
	public static event AuthorizationFinishAction LoggedIn;

	public delegate void LoginErrorAction(string errorText, string errorMessage);
	public static event LoginErrorAction OnLoginError;
	public static event LoginErrorAction OnRegisterError;

    public static string playfabId;
    public static bool connectedToPlayFab;

	
	public string accountNotFound = "That account could not be found.";
	public string accountBanned = "That account has been banned.";
	public string invalidPassword = "Password is invalid (6-24 characters).";
	public string invalidUsername = "Username is invalid (3-24 characters).";
	public string wrongPassword = "Wrong password for that user.";
	public string emailNotAvailable = "That email address is already taken.";
	public string emailNotValid = "Email address is invalid.";
	public string usernameNotAvailable = "That username is already taken.";

	private bool passwordsMatch = false;
    

    public void Login(string username, string password)
    {
         LoginToPlayfab(username, password);                
    }

    public void Register(string username, string password, string confirm, string email)
    {
		RegisterToPlayfab(username, password, confirm, email);
    }

    protected override void Initialize()
    {
        PlayFabData.TitleId = "663B";
        PlayFabSettings.TitleId = "663B";
        PhotonNetwork.autoJoinLobby = false;
    }
    

    private void LoginToPlayfab(string username, string password)
    {
		if(username.Length > 0 && password.Length > 0)
		{
           
            LoggingIn();
            LoginWithPlayFabRequest loginReq = new LoginWithPlayFabRequest();
	        loginReq.Username = username;
	        loginReq.Password = password;
	        loginReq.TitleId  = PlayFabData.TitleId;
	     
	        PlayFabClientAPI.LoginWithPlayFab(loginReq, OnLoginSuccess, OnPlayfabLoginError);
		}

    }

	private void RegisterToPlayfab(string username, string password, string confirm, string email)
	{
		if((password == confirm) && username.Length > 0 && password.Length > 0 && email.Length > 0)
		{
			RegisterPlayFabUserRequest registerReq = new RegisterPlayFabUserRequest();
			registerReq.Username = username;
			registerReq.Email    = email;
			registerReq.Password = password;
			registerReq.TitleId  = PlayFabData.TitleId;

			PlayFabClientAPI.RegisterPlayFabUser(registerReq, OnRegisterSuccess, OnPlayfabRegisterError);
		}
	}


    void OnLoginSuccess(LoginResult loginRes)
    {

        Debug.Log("Login Success");
        PlayFabData.AuthKey = loginRes.SessionTicket;
        playfabId = loginRes.PlayFabId;

        GetCloudScriptUrlRequest req = new GetCloudScriptUrlRequest();
        req.Testing = false;

        PlayFabClientAPI.GetCloudScriptUrl(req, null, OnPlayfabLoginError);

        connectedToPlayFab = true;

        LoggedIn();

    }

	void OnRegisterSuccess (RegisterPlayFabUserResult registerRes)
	{
		
		Debug.Log("Register Success");
		UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest ();
		request.DisplayName = registerRes.Username;
		PlayFabClientAPI.UpdateUserTitleDisplayName (request, NameUpdated, OnPlayfabRegisterError);
		OnRegisterError(registerRes.Username, "Successfully registered!");

	}

	public void NameUpdated(UpdateUserTitleDisplayNameResult result)
	{
		Debug.Log ("Name " + result.DisplayName + " updated");
	}

	void OnPlayfabLoginError(PlayFabError error)
	{
		if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
		{
			OnLoginError(error.ErrorMessage.ToString(), invalidPassword);
		}
		else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username"))
		{
			OnLoginError(error.ErrorMessage.ToString(), invalidUsername);
		}
		else if (error.Error == PlayFabErrorCode.AccountNotFound)
		{
			OnLoginError(error.ErrorMessage.ToString(), accountNotFound);
		}
		else if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			OnLoginError(error.ErrorMessage.ToString(), accountBanned);
		}
		else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
		{
			OnLoginError(error.ErrorMessage.ToString(), wrongPassword);
		}
		else
		{
			OnLoginError(error.Error.ToString(), error.ErrorMessage.ToString());
		}
	}

	void OnPlayfabRegisterError(PlayFabError error)
	{
		if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password")) || (error.Error == PlayFabErrorCode.InvalidPassword))
		{
			OnRegisterError(error.ErrorMessage.ToString(), invalidPassword);
		}
		else if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username")) || (error.Error == PlayFabErrorCode.InvalidUsername))
		{
			OnRegisterError(error.ErrorMessage.ToString(), invalidUsername);
		}
		else if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Email")) || (error.Error == PlayFabErrorCode.InvalidEmailAddress))
		{
			OnRegisterError(error.ErrorMessage.ToString(), emailNotValid);
		}
		else if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
		{
			OnRegisterError(error.ErrorMessage.ToString(), emailNotAvailable);
		}
		else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
		{
			OnRegisterError(error.ErrorMessage.ToString(), usernameNotAvailable);
		}
		else
		{
			OnRegisterError(error.Error.ToString(), error.ErrorMessage.ToString());
		}
	}

}
