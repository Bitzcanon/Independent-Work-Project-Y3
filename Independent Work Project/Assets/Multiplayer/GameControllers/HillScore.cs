using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class HillScore : MonoBehaviour
{
    private PhotonView PV;

    private float captureTimerBlue = 0.0f;
    private float captureTimerRed = 0.0f;

    private bool blueCaptureBlock;
    private bool redCaptureBlock;

    public int blueScore;
    public int redScore;

    public Text blueTextScore;
    public Text redTextScore;
    public Text contestedText;

    public GameObject winnerPanel;
    public Text winnerText;
    public bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        blueCaptureBlock = false;
        redCaptureBlock = false;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (blueCaptureBlock && redCaptureBlock) //Point is contested, do not increase timer
        {
            PV.RPC("RPC_ContestedText", RpcTarget.All, true);
            return;
        }
        else
        {
            PV.RPC("RPC_ContestedText", RpcTarget.All, false);
        }

        if (blueCaptureBlock == true)
        {
            captureTimerBlue += Time.deltaTime;
        }
        if (redCaptureBlock == true)
        {
            captureTimerRed += Time.deltaTime;
        }

        blueTextScore.text = "Blue Score: " + blueScore;
        redTextScore.text = "Red Score: " + redScore;
    }

    [PunRPC]
    void RPC_ContestedText(bool enabled)
    {
        if (enabled)
        {
            contestedText.gameObject.SetActive(true);
        }
        else
        {
            contestedText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!PV.IsMine)
            return;

        if (gameOver)
            return;

        if (blueCaptureBlock && redCaptureBlock)
        {
            return;
        }

        if (collision.gameObject.layer == 9) //Blue Team
        {
            if (collision.gameObject.GetComponent<PlayerSetup>().GetDeadStatus())
                return;

            PV.RPC("RPC_BlueCaptureBlock", RpcTarget.All, true);
            if (captureTimerBlue > 1f)
            {
                PV.RPC("RPC_AddScore", RpcTarget.All, false);
                captureTimerBlue = 0f;
            }
        }
        else if (collision.gameObject.layer == 10) //Red Team
        {
            if (collision.gameObject.GetComponent<PlayerSetup>().GetDeadStatus())
                return;

            PV.RPC("RPC_RedCaptureBlock", RpcTarget.All, true);
            if (captureTimerRed > 1f)
            {
                PV.RPC("RPC_AddScore", RpcTarget.All, true);
                captureTimerRed = 0f;
            }
        }
    }

    [PunRPC]
    void RPC_AddScore(bool team)
    {
        if (team == false)
        {
            blueScore += 1;
            if (blueScore >= 30)
            {
                winnerPanel.SetActive(true);
                winnerText.text = "Blue Team Wins!";
                gameOver = true;
            }
        }
        else if (team == true)
        {
            redScore += 1;
            if (redScore >= 30)
            {
                winnerPanel.SetActive(true);
                winnerText.text = "Red Team Wins!";
                gameOver = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PV.IsMine)
            return;

        if (gameOver)
            return;

        if (collision.gameObject.layer == 9)
        {
            PV.RPC("RPC_BlueCaptureBlock", RpcTarget.All, false);
        }
        else if (collision.gameObject.layer == 10)
        {
            PV.RPC("RPC_RedCaptureBlock", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void RPC_BlueCaptureBlock(bool enabled)
    {
        blueCaptureBlock = enabled;
    }

    [PunRPC]
    void RPC_RedCaptureBlock(bool enabled)
    {
        redCaptureBlock = enabled;
    }
}
