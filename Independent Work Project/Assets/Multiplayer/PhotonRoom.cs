using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //Room Info
    public static PhotonRoom room; //Singleton
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    //Player Info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    //Delayed Start
    public float startingTime;

    private bool readyToCount;
    private bool readyToStart;
    private float lessThanMaxPlayers; //When the lobby contains more than 1 person (including host), start countdown timer
    private float atMaxPlayers; //When max players are reached, start countdown timer from 5
    private float timeToStart;

    private void Awake()
    {
        //Set up singleton
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        //Subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        //Setting up eventListener to load code when starting new scene
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        //Initialize private variables
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 5;
        timeToStart = startingTime;
    }

    void Update()
    {
        //For delayed start only, count down to start
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                //Debug.Log("Display time to start to the players: " + timeToStart);
                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        //Sets player data when we join the room
        base.OnJoinedRoom();
        Debug.Log("We are now in a room!");

        photonPlayers = PhotonNetwork.PlayerList; //Save player list
        playersInRoom = photonPlayers.Length; //Save number of players in the room
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        //For delayed start only
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            //Change this Debug message with actual UI code to show in lobby
            Debug.Log("Players in room out of max players possible: (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        //For non-delayed start
        else
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log("Players in room out of max players possible: (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }

    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Called when multiplayer scene is laoded
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;
            //For delayed start game
            if (MultiplayerSettings.multiplayerSettings.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            //For non-delayed start game
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);

        //USE THIS FOR AI
        //PhotonNetwork.InstantiateSceneObject()
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //Display onto screen instead of Debug
        Debug.Log(otherPlayer.NickName + "has left the game!");
        playersInGame--;
    }
}
