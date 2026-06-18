using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RematchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    CombatManager combat_manager;

    [SerializeField]
    MenuController menu_controller;

    [SerializeField]
    GameObject host_rematch_tick, client_rematch_tick, highlight_block;

    [SerializeField]
    Color highlight_color;

    [SerializeField]
    TMP_Text button_text;

    PhotonView photon_view;

    bool host_ready = false;
    bool client_ready = false;

    private void Awake()
    {
        photon_view = GetComponent<PhotonView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!combat_manager.combat_over) return;

        if (PhotonNetwork.IsMasterClient)
        {
            photon_view.RPC("MarkRematchTick_RPC", RpcTarget.All, "master");
        }
        else
        {
            photon_view.RPC("MarkRematchTick_RPC", RpcTarget.All, "client");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!combat_manager.combat_over) return;

        button_text.color = Color.black;
        highlight_block.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!combat_manager.combat_over) return;

        button_text.color = highlight_color;
        highlight_block.SetActive(false);
    }

    [PunRPC]
    public void MarkRematchTick_RPC(string clicker)
    {
        if (clicker == "master")
        {
            Image img = host_rematch_tick.GetComponent<Image>();
            Color temp_color = img.color;
            temp_color.a = 1;
            img.color = temp_color;

            host_ready = true;
        }
        if (clicker == "client")
        {
            Image img = client_rematch_tick.GetComponent<Image>();
            Color temp_color = img.color;
            temp_color.a = 1;
            img.color = temp_color;

            client_ready = true;
        }

        if (host_ready && client_ready && PhotonNetwork.IsMasterClient)
        {
            menu_controller.photonView.RPC("ExecuteRematchReset_RPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void ClearRematchTicks_RPC()
    {
        host_rematch_tick.GetComponent<Image>().color = new Color(highlight_color.r, highlight_color.g, highlight_color.b, 0.3f);
        client_rematch_tick.GetComponent<Image>().color = new Color(highlight_color.r, highlight_color.g, highlight_color.b, 0.3f);
        button_text.color = new Color(highlight_color.r, highlight_color.g, highlight_color.b, 0.4f);

        host_ready = false;
        client_ready = false;
    }
}
