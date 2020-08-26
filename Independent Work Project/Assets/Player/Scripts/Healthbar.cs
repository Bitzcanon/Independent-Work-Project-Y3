using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class Healthbar : MonoBehaviour
{
    public GameObject playerAvatar;
    public PlayerSetup playerSetup;

    public Slider slider;
    public GameObject healthbarObject;
    private PhotonView PV;
    public Camera playerCamera;

    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Awake()
    {
        canvas.worldCamera = playerCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;
         
        PV.RPC("RPC_UpdateHealthBar", RpcTarget.All);
    }

    [PunRPC]
    void RPC_UpdateHealthBar()
    {
        healthbarObject.transform.position = playerAvatar.transform.position + new Vector3(0f, 3f, 0f);
        slider.value = playerSetup.health;
    }
}
