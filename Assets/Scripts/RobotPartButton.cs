using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using Photon.Pun;

public class RobotPartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    BotBuildManager robot_build_manager;

    [SerializeField]
    CombatManager combat_manager;

    [SerializeField]
    public RobotPart robot_part;

    [SerializeField]
    GameObject basic_info_panel;

    [SerializeField]
    GameObject detail_info_panel;

    string list_tick = "•";
    TMP_Text button_text;
    Button button;

    public Color button_highlight_color;

    void Start()
    {
        button_text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponentInChildren<Button>();

        if (robot_part)
        {
            button_text.text = list_tick + robot_part.name;
        }
        else
        {
            button_text.text = "----";
            button.interactable = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (combat_manager && combat_manager.combat_running) return;

        if (!robot_part)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            robot_build_manager.EquipPart(robot_build_manager.player1_bot, robot_part);
        }
        else
        {
            robot_build_manager.EquipPart(robot_build_manager.player2_bot, robot_part);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (combat_manager && combat_manager.combat_running) return;

        if (!robot_part)
        {
            return;
        }

        button_text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponentInChildren<Button>();

        TMP_Text basic_info_text = basic_info_panel.GetComponentInChildren<TextMeshProUGUI>();

        string type_text = "TYPE: " + robot_part.part_type + "\n";
        string name_text = "NAME: " + robot_part.name + "\n";
        string armor_text = "ARMOR: " + robot_part.armor + "\n";
        string integrity_text = "INTGRT: " + robot_part.integrity + "\n";

        basic_info_text.text = type_text + name_text + armor_text + integrity_text;
        button_text.color = new Color(0, 0, 0, 1);
        Image button_image = GetComponentInChildren<Image>();
        button_image.color = button_highlight_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (combat_manager && combat_manager.combat_running) return;

        Image button_image = GetComponentInChildren<Image>();
        button_text = GetComponentInChildren<TextMeshProUGUI>();

        Color button_color = new Color(button_highlight_color.r, button_highlight_color.g, button_highlight_color.b, 0f);
        button_image.color = button_color;
        button_text.color = button_highlight_color;
    }
}
