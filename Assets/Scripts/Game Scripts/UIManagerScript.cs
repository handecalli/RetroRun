using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

	public Animator startButton;
	public Animator settingsButton;
	public Animator settingsPanel;
	public Animator contentPanel;
	public Animator gearImage;
	public Animator aboutPanel;
	public Animator leaderboardPanel;
	public Animator menuPanel;
	public Animator marketPanel;

	public GameObject loadingScreen;
    public GameObject musicObject;
    public Toggle toggleMusic;
    public Toggle toggleGameSound;
    public AudioSource musicSource;

	private string lobby = "Lobby";
    private bool isMusicMute = false;
    private bool isGameSoundMute = false;
	private bool isLoading;

    void Awake()
    {
        musicObject = GameObject.Find("Undestroyable");
        musicSource = musicObject.GetComponent<AudioSource>();

        if (PlayerPrefs.GetString("isGameSoundMute") == "True")
        {
            toggleGameSound.isOn = true;
        }

        if (PlayerPrefs.GetString("isMusicMute") == "True")
        {
            isMusicMute = false;
            toggleMusic.isOn = true;
        }
    }

	void Start() 
	{
		isLoading = true;

		RectTransform transform = contentPanel.gameObject.transform as RectTransform;        
		Vector2 position = transform.anchoredPosition;
		position.y -= transform.rect.height;
		transform.anchoredPosition = position;
	}

	void Update()
	{
		if (isLoading && Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel("Login");
		}
	}

	public void OpenSettings() {
		
		settingsPanel.enabled = true;
		settingsPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}

	public void CloseSettings() {

		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
	}

	public void OpenAbout() {
		
		aboutPanel.enabled = true;
		aboutPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
	}
	
	public void CloseAbout() {
		
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}
	
	public void OpenLeaderboard() {
		
		leaderboardPanel.enabled = true;
		leaderboardPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}

	public void CloseLeaderboard() {
		
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
	}

	public void ToggleMenu() {

		contentPanel.enabled = true;
		bool isHidden = contentPanel.GetBool("isHidden");
		contentPanel.SetBool("isHidden", !isHidden);

		gearImage.enabled = true;
		gearImage.SetBool("isHidden", !isHidden);
	}

	
	public void StartGame() {

        CatalogScript.catalogLoaded = false;
		Application.LoadLevel(lobby);   
     
	}

	public void GoToMarket() {

		menuPanel.enabled = true;
		marketPanel.enabled = true;
		marketPanel.SetBool("isHidden", false);
		menuPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		gearImage.SetBool("isHidden", true);
	}
	
	public void GoToMenu() {
		
		menuPanel.enabled = true;
		marketPanel.enabled = true;
		marketPanel.SetBool("isHidden", true);
		menuPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
	}
	
	void OnEnable()
	{
		MarketItemScript.InitializeDone += CloseLoading;
	}
	
	void OnDisable()
	{
		MarketItemScript.InitializeDone -= CloseLoading;
	}

	public void CloseLoading ()
	{
		loadingScreen.SetActive(false);
		isLoading = false;
	}

	public void ShowLoading ()
	{
		loadingScreen.SetActive(true);
		isLoading = true;
	}

	public void LogOut() {
        CatalogScript.catalogLoaded = false;

        Destroy(musicObject);

        PlayerPrefs.DeleteKey("prevUsername");
        PlayerPrefs.DeleteKey("prevPassword");

        Debug.Log("PlayerPrefs deleted.");
		Application.LoadLevel("Login");
	}

    public void OnMusicMuteClick()
    {
        
        if (!isMusicMute)
        {
            isMusicMute = true;
            musicSource.mute = true;
            PlayerPrefs.SetString("isMusicMute", isMusicMute.ToString());
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Delete isMute from PlayerPrefs");

            isMusicMute = false;
            PlayerPrefs.DeleteKey("isMusicMute");
            musicSource.mute = false;
        }
    }

    public void OnGameSoundMuteClick()
    {
        if (!isGameSoundMute)
        {
            isGameSoundMute = true;
            PlayerPrefs.SetString("isGameSoundMute", isGameSoundMute.ToString());
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Delete isGameSoundMute from PlayerPrefs");

            isGameSoundMute = false;
            PlayerPrefs.DeleteKey("isGameSoundMute");
        }
    }
}
