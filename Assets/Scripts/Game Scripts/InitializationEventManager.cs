using UnityEngine;
using System.Collections;

public class InitializationEventManager {

    public delegate void GenericGameEvent();

    public static GenericGameEvent OnNextInitialization;

    public static void TriggerNextInitialization () {

        if (OnNextInitialization != null) {

            OnNextInitialization();
        
        }
 
    }

}
