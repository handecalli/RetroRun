using UnityEngine;
using System.Collections;

public class AudioEventManager  {

    public static event PlaySoundIDEvent OnPlaySoundID;
    public delegate void PlaySoundIDEvent(string soundID);

    public static void TriggerPlaySoundID(string soundID) {

        if (OnPlaySoundID != null) {

            OnPlaySoundID(soundID);
        
        }
    
    }
}
