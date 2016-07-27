using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {

    private AudioSource music;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        music = GetComponent<AudioSource>();
        music.playOnAwake = !(PlayerPrefs.GetString("isMute") == "True");

    }

    void OnEnable()
    {
        Application.LoadLevel("Menu");
    }

}
