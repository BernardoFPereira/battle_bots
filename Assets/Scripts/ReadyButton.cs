using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using Photon.Pun;

public class ReadyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    GameObject highlight_object;

    [SerializeField]
    Color highlight_color;

    [SerializeField]
    BotBuildManager robot_build_manager;

    [SerializeField]
    bool is_master_button, is_client_button;

    Image button_highlight;
    TMP_Text button_text;

    PhotonView photon_view;

    public void Start()
    {
        button_text = GetComponent<TextMeshProUGUI>();
        button_highlight = highlight_object.GetComponentInChildren<Image>();
        photon_view = GetComponent<PhotonView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        button_highlight.color = Color.black;
        button_text.color = highlight_color;

        if (PhotonNetwork.IsMasterClient && is_master_button)
        {
            if (!robot_build_manager.player1_bot.is_built)
            {
                return;
            }
            robot_build_manager.player1_bot.is_ready = !robot_build_manager.player1_bot.is_ready;
            // button_text.text = robot_build_manager.player1_bot.is_ready ? "*** READY ***" : "READY";

            photon_view.RPC("UpdateReadyStatus_RPC", RpcTarget.AllBuffered, robot_build_manager.player1_bot.is_ready);
        }
        if (!PhotonNetwork.IsMasterClient && is_client_button)
        {
            if (!robot_build_manager.player2_bot.is_built)
            {
                return;
            }
            robot_build_manager.player2_bot.is_ready = !robot_build_manager.player2_bot.is_ready;
            // button_text.text = robot_build_manager.player2_bot.is_ready ? "*** READY ***" : "READY";

            photon_view.RPC("UpdateReadyStatus_RPC", RpcTarget.AllBuffered, robot_build_manager.player2_bot.is_ready);
        }
        // button_text.text = "*** READY ***";
    }

    [PunRPC]
    public void UpdateReadyStatus_RPC(bool is_ready)
    {
        button_text.text = is_ready ? "*** READY ***" : "READY";

        if (is_master_button)
        {
            robot_build_manager.player1_bot.is_ready = is_ready;
        }
        if (is_client_button)
        {
            robot_build_manager.player2_bot.is_ready = is_ready;
        }
        // button_text.text = "*** READY ***";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PhotonNetwork.IsMasterClient && is_master_button)
        {
            button_highlight.color = highlight_color;
            button_text.color = Color.black;
        }
        if (!PhotonNetwork.IsMasterClient && is_client_button)
        {
            button_highlight.color = highlight_color;
            button_text.color = Color.black;
        }
        else { return; }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PhotonNetwork.IsMasterClient && is_master_button)
        {
            button_highlight.color = Color.black;
            button_text.color = highlight_color;
        }
        if (!PhotonNetwork.IsMasterClient && is_client_button)
        {
            button_highlight.color = Color.black;
            button_text.color = highlight_color;
        }
        else { return; }
    }
}
