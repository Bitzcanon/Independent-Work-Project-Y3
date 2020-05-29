using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    public PhotonView PV;
    public GameObject avatar;

    public int team;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        if (avatar == null && team != 0)
        {
            if (team == 1)
            {
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPointsTeamBlue.Length);
                if (PV.IsMine)
                {
                    avatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Knight"), GameSetup.GS.spawnPointsTeamBlue[spawnPicker].position, GameSetup.GS.spawnPointsTeamBlue[spawnPicker].rotation, 0);
                    Debug.Log("Created Player");
                }
            }
            else
            {
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPointsTeamRed.Length);
                if (PV.IsMine)
                {
                    avatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Knight"), GameSetup.GS.spawnPointsTeamRed[spawnPicker].position, GameSetup.GS.spawnPointsTeamRed[spawnPicker].rotation, 0);
                    Debug.Log("Created Player");
                }
            }
        }

    }

    [PunRPC]
    void RPC_GetTeam()
    {
        team = GameSetup.GS.nextPlayersTeam;
        GameSetup.GS.UpdateTeam();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, team);
    }

    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        team = whichTeam;
    }
}
