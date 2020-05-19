//using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    //private PhotonView PV;

    //Public Variables
    //public static PlayerInfo PI;

    //public int selectedCharacter;

    public int health;
    public bool team; //0 for blue team, 1 for red team

    //public Camera playerCamera;
    //public AudioListener audioListener;

    // Start is called before the first frame update
    void Start()
    {
       
        //if (PlayerPrefs.HasKey("Character"))
        //{
        //    selectedCharacter = PlayerPrefs.GetInt("Character");
        //}
        //else
        //{
        //    selectedCharacter = 0;
        //    PlayerPrefs.SetInt("Character", selectedCharacter);
        //}

    }

    private void OnEnable()
    {
        //if (PlayerInfo.PI == null)
        //{
        //    PlayerInfo.PI = this;
        //}
        //else
        //{
        //    if (PlayerInfo.PI != this)
        //    {
        //        Destroy(PlayerInfo.PI.gameObject);
        //        PlayerInfo.PI = this;
        //    }
        //}
    }
}
