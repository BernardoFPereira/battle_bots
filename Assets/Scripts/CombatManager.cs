using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text combat_log;

    [SerializeField]
    Robot player1_bot, player2_bot;

    PhotonView photon_view;

    public bool combat_running = false;
    bool combat_over = false;

    int player1_bot_integrity;
    int player2_bot_integrity;

    int player1_current_integrity;
    int player2_current_integrity;

    private void Awake()
    {
        photon_view = GetComponent<PhotonView>();
    }

    public void StartCombatLoop()
    {
        combat_log.text += "--- Combat Started! ---";

        if (!PhotonNetwork.IsMasterClient) return;

        combat_running = true;
        combat_over = false;

        player1_bot_integrity = CalculateTotalIntegrity(player1_bot);
        player2_bot_integrity = CalculateTotalIntegrity(player2_bot);

        StartCoroutine(CombatLoopRoutine());
    }

    private int CalculateTotalIntegrity(Robot robot)
    {
        return (robot.frame.integrity + robot.head.integrity + robot.right_arm.integrity + robot.left_arm.integrity + robot.locomotion.integrity);
    }

    private IEnumerator CombatLoopRoutine()
    {
        while (!combat_over)
        {
            float player1_initiative = Random.Range(0f, player1_bot.locomotion.base_speed);
            float player2_initiative = Random.Range(0f, player2_bot.locomotion.base_speed);

            photon_view.RPC("ExecuteCombatRound_RPC", RpcTarget.AllBuffered, player1_initiative, player2_initiative);
            CheckCombatStatus();

            yield return new WaitForSeconds(2.0f);
        }
    }

    [PunRPC]
    private void ExecuteCombatRound_RPC(float init_1, float init_2)
    {
        if (init_1 >= init_2)
        {
            // combat_log.text += $"\nPlayer 1 attacks!";
            combat_log.text += "\n<color=#6495ED>" + player1_bot.name + "</color> attacks!";
        }
        else
        {
            combat_log.text += "\n<color=yellow>" + player2_bot.name + "</color> attacks!";
            // combat_log.text += $"\nPlayer 2 attacks!";
        }
    }

    private void CheckCombatStatus()
    {
        if (player1_bot.frame.integrity <= 0 || player2_bot.frame.integrity <= 0)
        {
            combat_over = true;
        }
    }
}
