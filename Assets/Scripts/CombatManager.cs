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
    int player1_bot_armor;
    int player2_bot_armor;

    int player1_current_integrity;
    int player2_current_integrity;
    int player1_current_armor;
    int player2_current_armor;

    string host_color = "#6495ED";
    string client_color = "yellow";

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
        player1_bot_armor = CalculateTotalArmor(player1_bot);
        player2_bot_armor = CalculateTotalArmor(player2_bot);

        player1_current_integrity = player1_bot_integrity;
        player2_current_integrity = player2_bot_integrity;
        player1_current_armor = player1_bot_armor;
        player2_current_armor = player2_bot_armor;

        StartCoroutine(CombatLoopRoutine());
    }

    private int CalculateTotalIntegrity(Robot robot)
    {
        return (robot.frame.integrity + robot.head.integrity + robot.right_arm.integrity + robot.left_arm.integrity + robot.locomotion.integrity);
    }

    private int CalculateTotalArmor(Robot robot)
    {
        return (robot.frame.armor + robot.head.armor + robot.right_arm.armor + robot.left_arm.armor + robot.locomotion.armor);
    }

    private IEnumerator CombatLoopRoutine()
    {
        string winner = "";
        while (!combat_over)
        {
            int player1_initiative = Random.Range(0, player1_bot.locomotion.base_speed);
            int player2_initiative = Random.Range(0, player2_bot.locomotion.base_speed);

            ExecuteCombatRound(player1_initiative, player2_initiative);

            winner = CheckCombatStatus();

            yield return new WaitForSeconds(1.5f);
        }
        photon_view.RPC("ConcludeCombat_RPC", RpcTarget.All, winner);
    }

    private string CheckCombatStatus()
    {
        if (player1_current_integrity <= 0)
        {
            combat_over = true;
            return "<color=" + client_color + ">" + player2_bot.name + "</color>";
        }
        else if (player2_current_integrity <= 0)
        {
            combat_over = true;
            return "<color=" + host_color + ">" + player1_bot.name + "</color>";
        }
        return "";
    }

    public void ExecuteCombatRound(int init_1, int init_2)
    {
        Robot attacker = init_1 >= init_2 ? player1_bot : player2_bot;
        string attacker_color = init_1 >= init_2 ? host_color : client_color;

        int potential_dmg = 0;
        string arm_used = "";

        switch (Random.Range(0, 2))
        {
            case 0:
                if (attacker.right_arm.weapon_range != WeaponRange.NONE)
                {
                    potential_dmg = attacker.right_arm.damage;
                    arm_used = attacker.right_arm.name;
                }
                else
                {
                    potential_dmg = attacker.left_arm.damage;
                    arm_used = attacker.left_arm.name;
                }
                break;
            case 1:
                if (attacker.left_arm.weapon_range != WeaponRange.NONE)
                {
                    potential_dmg = attacker.left_arm.damage;
                    arm_used = attacker.left_arm.name;
                }
                else
                {
                    potential_dmg = attacker.right_arm.damage;
                    arm_used = attacker.right_arm.name;
                }
                break;
        }

        if (init_1 >= init_2) // player 1 attacking
        {
            if (player2_current_armor <= 0)
            {
                player2_current_integrity -= potential_dmg;
            }
            else
            {
                player2_current_armor -= potential_dmg;
            }
        }
        else // player 2 attacking
        {
            if (player1_current_armor <= 0)
            {
                player1_current_integrity -= potential_dmg;
            }
            else
            {
                player1_current_armor -= potential_dmg;
            }
        }

        photon_view.RPC("BroadcastCombatRound_RPC", RpcTarget.AllBuffered,
            attacker.name, attacker_color, arm_used, potential_dmg,
            player1_current_armor, player1_current_integrity,
            player2_current_armor, player2_current_integrity);
    }

    [PunRPC]
    public void BroadcastCombatRound_RPC(string attacker_name, string attacker_color, string arm_used, int damage,
        int p1_armor, int p1_integrity, int p2_armor, int p2_integrity)
    {
        player1_current_armor = p1_armor;
        player1_current_integrity = p1_integrity;
        player2_current_armor = p2_armor;
        player2_current_integrity = p2_integrity;

        combat_log.text += "\n<color=" + attacker_color + ">" + attacker_name + "</color>";
        combat_log.text += " attacks with " + arm_used + " for " + damage + " damage!";
    }

    [PunRPC]
    public void ConcludeCombat_RPC(string combat_winner)
    {
        combat_log.text += "\n--- COMBAT FINISHED! ---";
        combat_log.text += "\nThe winner is: " + combat_winner;
    }
}
