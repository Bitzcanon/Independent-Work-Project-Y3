using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject avatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        //Temporary spawning
        int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
        if (PV.IsMine)
        {
            avatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Knight"), GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
            Debug.Log("Created Player");
        }
    }
}
