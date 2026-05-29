using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReadyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    GameObject highlight_object;

    [SerializeField]
    Color highlight_color;

    [SerializeField]
    BotBuildManager robot_build_manager;

    Image button_highlight;
    TMP_Text button_text;

    private void Start()
    {
        button_text = GetComponent<TextMeshProUGUI>();
        button_highlight = highlight_object.GetComponentInChildren<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        button_highlight.color = Color.black;
        button_text.color = highlight_color;
        if (!robot_build_manager.player1_bot.is_built)
        {
            return;
        }
        robot_build_manager.player1_bot.is_ready = !robot_build_manager.player1_bot.is_ready;
        button_text.text = robot_build_manager.player1_bot.is_ready ? "*** READY ***" : "READY";
        // button_text.text = "*** READY ***";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button_text.color = Color.black;
        button_highlight.color = highlight_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button_highlight.color = Color.black;
        button_text.color = highlight_color;
    }
}
