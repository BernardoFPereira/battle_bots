using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField chat_input;

    [SerializeField]
    TMP_Text chat_log;

    public void SendInputToChat()
    {
        // chat_log.text += "\n" + "• " + chat_input.text;
        string sender = PhotonNetwork.NickName[0].ToString().ToUpper();
        if (!PhotonNetwork.IsMasterClient)
        {
            sender = "<color=yellow>" + sender + "</color>";
        }
        else
        {
            sender = "<color=#6495ED>" + sender + "</color>";
        }
        GetComponent<PhotonView>().RPC("WriteToChat_RPC", RpcTarget.AllBuffered, sender, chat_input.text);
        chat_input.text = "";
        // chat_input.ActivateInputField();
    }

    [PunRPC]
    public void WriteToChat_RPC(string sender, string to_send)
    {
        chat_log.text += "\n" + "•" + sender + " " + to_send;
        // chat_input.text = "";
    }
}
