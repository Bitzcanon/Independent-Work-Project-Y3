using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private PhotonView PV;
    public int characterValue;
    public GameObject character;

    public int health;
    public Camera playerCamera;
    public AudioListener audioListener;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            //PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.selectedCharacter);
        }
        else
        {
            Destroy(playerCamera);
            Destroy(audioListener);
        }
    }

    void Update()
    {
        //if (health <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Damage Taken!");
    }
}
