using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSettings;

    //Room Info
    public bool delayStart;
    public int maxPlayers;

    //Build order for scenes
    public int menuScene;
    public int multiplayerScene;

    private void Awake()
    {
        //Initialize Singleton
        if (MultiplayerSettings.multiplayerSettings == null)
        {
            MultiplayerSettings.multiplayerSettings = this;
        }
        else
        {
            if (MultiplayerSettings.multiplayerSettings != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
