using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviour
{
    private PhotonView PV;
    public int characterValue;
    public GameObject character;

    public int health;
    public bool isDead = false;
    public Camera playerCamera;
    public AudioListener audioListener;

    public float respawning = 3f;
    public Text respawnTimer;

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
        if (!PV.IsMine)
            return;

        if (isDead)
        {
            respawning -= Time.deltaTime;
            respawnTimer.enabled = true;
            RespawnTimer();
            if (respawning <= 0)
            {
                respawning = 3f;
                respawnTimer.enabled = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            StartCoroutine(RespawnPlayer());
        }

        Debug.Log("Damage Taken!");
    }

    public bool GetDeadStatus()
    {
        return isDead;
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f);

        health = 100;
        isDead = false;

        int spawnPicker = Random.Range(0, GameSetup.GS.spawnPointsTeamBlue.Length);

        if (this.gameObject.layer == 9) //Blue team
        {
            this.gameObject.transform.position = GameSetup.GS.spawnPointsTeamBlue[spawnPicker].position;
            this.gameObject.transform.rotation = GameSetup.GS.spawnPointsTeamBlue[spawnPicker].rotation;
        }
        else if (this.gameObject.layer == 10)
        {
            this.gameObject.transform.position = GameSetup.GS.spawnPointsTeamRed[spawnPicker].position;
            this.gameObject.transform.rotation = GameSetup.GS.spawnPointsTeamRed[spawnPicker].rotation;
        }
    }

    void RespawnTimer()
    {
        respawnTimer.text = "You Died!\n" + respawning; 
    }
}
