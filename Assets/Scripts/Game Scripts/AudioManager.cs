using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AudioType
{

    public string id;
    public AudioClip clip;

}
public class AudioManager : MonoBehaviour {


    public int channelNumber;
    public AudioType[] audioDB;

    private List<AudioSource> channels;

    private bool isGameSoundMute = false;

    void Awake()
    {
        if (PlayerPrefs.GetString("isGameSoundMute") == "True")
        {
            gameObject.SetActive(false);
            isGameSoundMute = true;
        }
    }

    void Start()
    {
        Debug.Log("audioDB length: " + audioDB.Length);
        channels = new List<AudioSource>();

        for (int channelIndex = 0; channelIndex < channelNumber; channelIndex++) 
        {
            GameObject channelObject = new GameObject();
            Transform oTransform = channelObject.transform;
            oTransform.parent = transform;
            oTransform.localPosition = Vector3.zero;
            oTransform.localRotation = Quaternion.identity;

            AudioSource channel = channelObject.AddComponent<AudioSource>() as AudioSource;
            channels.Add(channel);
        }
    }

    void OnEnable()
    {
        if (!isGameSoundMute)
        {
            AudioEventManager.OnPlaySoundID += OnPlaySound;
        }
    }

    void OnDisable()
    {
        if (!isGameSoundMute)
        {
            AudioEventManager.OnPlaySoundID -= OnPlaySound;
        }
    }

    void OnPlaySound(string soundID)
    {
        AudioSource aSource = GetAvailableChannel();

        if (aSource == null)
        {
            Debug.Log("Not enough free channels");
        }

        AudioClip clip = GetClip(soundID);

        if (clip == null)
        {
            Debug.Log("Audio Clip does not exist");
        }

        aSource.clip = clip;
        aSource.Play();

    }

    private AudioClip GetClip(string ID) {

        for (int idIndex = audioDB.Length - 1; idIndex > -1; idIndex--) {

            if (audioDB[idIndex].id == ID) {

                return audioDB[idIndex].clip;
            
            }
        
        }

        Debug.LogWarning("Audio Clip with designated ID doesn't exist!");
        return null;
    
    }

    private AudioSource GetAvailableChannel()
    {

        foreach ( AudioSource aSource in channels ) {

            if (!aSource.isPlaying) {

                return aSource;
            
            }
        
        }

        Debug.LogWarning("No free audio channels!!!");
        return null;
    
    }

}
