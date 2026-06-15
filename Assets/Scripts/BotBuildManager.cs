using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BotBuildManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Robot player1_bot;

    [SerializeField]
    public Robot player2_bot;

    [SerializeField]
    public TMP_Text host_robot_name_plate_text;

    [SerializeField]
    public TMP_Text host_player_parts;

    [SerializeField]
    public TMP_Text host_player_stats;

    [SerializeField]
    public TMP_Text client_robot_name_plate_text;

    [SerializeField]
    public TMP_Text client_player_parts;

    [SerializeField]
    public TMP_Text client_player_stats;

    [SerializeField]
    BattlefieldManager battlefield_manager;

    public PhotonView photon_view;

    [SerializeField]
    RobotPart[] part_registry;

    private void Awake()
    {
        photon_view = GetComponent<PhotonView>();
    }

    public void Start()
    {

        ClearButtonTicks();
        // UpdatePartsInfo(player1_bot);
        // BuildTestEnemy();
        // UpdatePartsInfo(player2_bot);
        // player2_bot.is_built = true;
        // player2_bot.is_ready = true;
    }

    // void BuildTestEnemy()
    // {
    //     player2_bot.equipped_parts["frame"] = player2_bot.frame;
    //     player2_bot.equipped_parts["head"] = player2_bot.head;
    //     player2_bot.equipped_parts["r_arm"] = player2_bot.right_arm;
    //     player2_bot.equipped_parts["l_arm"] = player2_bot.left_arm;
    //     player2_bot.equipped_parts["locomotion"] = player2_bot.locomotion;
    // }

    public void EquipPart(Robot robot, RobotPart part)
    {
        switch (part.part_type)
        {
            case PartType.FRAME:
                RobotFrame frame_to_equip = part as RobotFrame;
                if (robot.frame)
                {
                    if (frame_to_equip == robot.frame)
                    {
                        Debug.Log("Already Equipped, removing!!");
                        robot.equipped_parts["frame"] = null;
                        break;
                    }
                }
                robot.equipped_parts["frame"] = frame_to_equip;
                break;
            case PartType.HEAD:
                RobotPartHead head_to_equip = part as RobotPartHead;
                if (robot.head)
                {
                    if (head_to_equip == robot.head)
                    {
                        Debug.Log("Already Equipped, removing!!");
                        robot.equipped_parts["head"] = null;
                        break;
                    }
                }
                robot.equipped_parts["head"] = head_to_equip;
                break;
            case PartType.ARM:
                RobotPartArm arm_to_equip = part as RobotPartArm;
                if (robot.right_arm)
                {
                    if (arm_to_equip == robot.right_arm)
                    {
                        Debug.Log("Already Equipped, removing!!");
                        robot.equipped_parts["r_arm"] = null;
                        break;
                    }
                }
                else
                {
                    robot.equipped_parts["r_arm"] = arm_to_equip;
                    break;
                }
                if (robot.left_arm)
                {
                    if (arm_to_equip == robot.left_arm)
                    {
                        Debug.Log("Already Equipped, removing!!");
                        robot.equipped_parts["l_arm"] = null;
                        break;
                    }
                }
                else
                {
                    robot.equipped_parts["l_arm"] = arm_to_equip;
                    break;
                }
                if (robot.right_arm && robot.left_arm)
                {
                    robot.equipped_parts["r_arm"] = arm_to_equip;
                    break;
                }
                break;
            case PartType.LOCOMOTION:
                RobotPartLocomotion locomotion_to_equip = part as RobotPartLocomotion;
                if (robot.locomotion)
                {
                    if (locomotion_to_equip == robot.locomotion)
                    {
                        Debug.Log("Already Equipped, removing!!");
                        robot.equipped_parts["locomotion"] = null;
                        robot.locomotion = null;
                        break;
                    }
                }
                robot.equipped_parts["locomotion"] = locomotion_to_equip;
                break;
        }

        UpdateButtonTicks(robot);
        UpdateRobotName(robot);
        UpdateRobotData(robot);
        // UpdateRobotStatsDisplay(robot);
        // UpdatePartsInfo(robot);
        // robot.is_built = CheckRobotBuildComplete(robot);
    }

    void ClearButtonTicks()
    {
        GameObject[] equip_ticks = GameObject.FindGameObjectsWithTag("EquipTick");
        foreach (GameObject tick in equip_ticks)
        {
            Image tick_img = tick.GetComponent<Image>();
            tick_img.enabled = false;
        }
    }

    // public void UpdateRobotName(Robot robot)
    // {
    //     string name_plate_fmt = "• ROBOT\n";

    //     if (PhotonNetwork.IsMasterClient)
    //     {
    //         string bot_name = name_plate_fmt + robot.name;
    //         host_robot_name_plate_text.text = bot_name;
    //     }
    //     else
    //     {
    //         string bot_name = name_plate_fmt + robot.name;
    //         client_robot_name_plate_text.text = bot_name;
    //     }
    // }

    public bool CheckRobotBuildComplete(Robot robot)
    {
        return (
            robot.equipped_parts["frame"] &&
            robot.equipped_parts["head"] &&
            robot.equipped_parts["r_arm"] &&
            robot.equipped_parts["l_arm"] &&
            robot.equipped_parts["locomotion"]
        );
    }

    void UpdateRobotStatsDisplay(Robot robot)
    {
        string robot_range = "NONE";
        int robot_integrity_sum = 0;
        int robot_armor = 0;
        int robot_speed = 0;

        int frame_integrity = robot.equipped_parts["frame"] ? robot.equipped_parts["frame"].integrity : 0;
        int head_integrity = robot.equipped_parts["head"] ? robot.equipped_parts["head"].integrity : 0;
        int r_arm_integrity = robot.equipped_parts["r_arm"] ? robot.equipped_parts["r_arm"].integrity : 0;
        int l_arm_integrity = robot.equipped_parts["l_arm"] ? robot.equipped_parts["l_arm"].integrity : 0;
        int locomotion_integrity = robot.equipped_parts["locomotion"] ? robot.equipped_parts["locomotion"].integrity : 0;

        int frame_armor = robot.equipped_parts["frame"] ? robot.equipped_parts["frame"].armor : 0;
        int head_armor = robot.equipped_parts["head"] ? robot.equipped_parts["head"].armor : 0;
        int r_arm_armor = robot.equipped_parts["r_arm"] ? robot.equipped_parts["r_arm"].armor : 0;
        int l_arm_armor = robot.equipped_parts["l_arm"] ? robot.equipped_parts["l_arm"].armor : 0;
        int locomotion_armor = robot.equipped_parts["locomotion"] ? robot.equipped_parts["locomotion"].armor : 0;

        robot_integrity_sum = frame_integrity + head_integrity + r_arm_integrity + l_arm_integrity + locomotion_integrity;
        robot_armor = frame_armor + head_armor + r_arm_armor + l_arm_armor + locomotion_armor;

        RobotPartArm r_arm = robot.equipped_parts["r_arm"] as RobotPartArm;
        RobotPartArm l_arm = robot.equipped_parts["l_arm"] as RobotPartArm;

        WeaponRange r_arm_range = robot.equipped_parts["r_arm"] ? r_arm.weapon_range : WeaponRange.NONE;
        WeaponRange l_arm_range = robot.equipped_parts["l_arm"] ? l_arm.weapon_range : WeaponRange.NONE;

        if (r_arm_range == l_arm_range)
        {
            robot_range = r_arm_range.ToString();
        }
        if (r_arm_range > l_arm_range)
        {
            robot_range = r_arm_range.ToString();
        }
        if (r_arm_range < l_arm_range)
        {
            robot_range = l_arm_range.ToString();
        }

        if (robot.equipped_parts["locomotion"])
        {
            RobotPartLocomotion locomotion_part;
            locomotion_part = robot.equipped_parts["locomotion"] as RobotPartLocomotion;
            robot_speed = locomotion_part.base_speed;

            switch (battlefield_manager.selected_arena.terrain)
            {
                case TerrainType.SMOOTH:
                    switch (locomotion_part.locomotion_type)
                    {
                        case LocomotionType.LEGS:
                            robot_speed += 5;
                            break;
                        case LocomotionType.WHEEL:
                            robot_speed += 10;
                            break;
                        case LocomotionType.TRACKS:
                            robot_speed -= 5;
                            break;
                    }
                    break;
                case TerrainType.IRREGULAR:
                    switch (locomotion_part.locomotion_type)
                    {
                        case LocomotionType.LEGS:
                            robot_speed += 5;
                            break;
                        case LocomotionType.WHEEL:
                            robot_speed -= 5;
                            break;
                        case LocomotionType.TRACKS:
                            robot_speed += 5;
                            break;
                    }
                    break;
                case TerrainType.ROCKY:
                    switch (locomotion_part.locomotion_type)
                    {
                        case LocomotionType.LEGS:
                            robot_speed -= 5;
                            break;
                        case LocomotionType.WHEEL:
                            robot_speed -= 10;
                            break;
                        case LocomotionType.TRACKS:
                            robot_speed += 5;
                            break;
                    }
                    break;
            }
        }

        string range_text = "• EF.RANGE: " + robot_range + "\n";
        string integrity_text = "• INTGT: " + robot_integrity_sum + "\n";
        string armor_text = "• ARMOR: " + robot_armor + "\n";
        string speed_text = "• SPEED: " + robot_speed + "\n";

        string updated_stat_block = range_text + integrity_text + armor_text + speed_text;

        if (robot == player1_bot)
        {
            host_player_stats.text = updated_stat_block;
        }
        else if (robot == player2_bot)
        {
            client_player_stats.text = updated_stat_block;
        }

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     photon_view.RPC("UpdateHostStatBlock_RPC", RpcTarget.AllBuffered, updated_stat_block);
        // }
        // else
        // {
        //     photon_view.RPC("UpdateClientStatBlock_RPC", RpcTarget.AllBuffered, updated_stat_block);
        // }
    }

    [PunRPC]
    void UpdateHostStatBlock_RPC(string updated_stats)
    {
        host_player_stats.text = updated_stats;
    }

    [PunRPC]
    void UpdateClientStatBlock_RPC(string updated_stats)
    {
        client_player_stats.text = updated_stats;
    }

    void UpdateButtonTicks(Robot robot)
    {
        Debug.Log("UPDATING EQUIPMENT TICKS!");
        GameObject[] equip_ticks = GameObject.FindGameObjectsWithTag("EquipTick");
        foreach (GameObject tick in equip_ticks)
        {
            Image tick_img = tick.GetComponent<Image>();
            tick_img.enabled = false;
            RobotPartButton part_button = tick.GetComponentInParent<RobotPartButton>();
            RobotPart part_on_button = part_button.robot_part;
            if (part_on_button && robot.equipped_parts.ContainsValue(part_on_button))
            {
                tick_img.enabled = true;
            }
        }
    }

    public void UpdatePartsInfo(Robot robot)
    {
        string name_plate_fmt = "• ROBOT\n";
        string list_tick = "• ";

        string frame = robot.equipped_parts["frame"] ? list_tick + "FR: " + robot.equipped_parts["frame"].name + "\n" : list_tick + "FR: <color=red>NONE\n</color>";
        string head = robot.equipped_parts["head"] ? list_tick + "HD: " + robot.equipped_parts["head"].name + "\n" : list_tick + "HD: <color=red>NONE</color>\n";
        string r_arm = robot.equipped_parts["r_arm"] ? list_tick + "AR: " + robot.equipped_parts["r_arm"].name + "\n" : list_tick + "AR: <color=red>NONE</color>\n";
        string l_arm = robot.equipped_parts["l_arm"] ? list_tick + "AL: " + robot.equipped_parts["l_arm"].name + "\n" : list_tick + "AL: <color=red>NONE</color>\n";
        string locomotion = robot.equipped_parts["locomotion"] ? list_tick + "LOC: " + robot.equipped_parts["locomotion"].name : list_tick + "LOC: <color=red>NONE</color>";

        string bot_name = name_plate_fmt + robot.name;
        string bot_parts = frame + head + r_arm + l_arm + locomotion;

        if (robot == player1_bot)
        {
            host_robot_name_plate_text.text = bot_name;
            host_player_parts.text = bot_parts;
        }
        else if (robot == player2_bot)
        {
            client_robot_name_plate_text.text = bot_name;
            client_player_parts.text = bot_parts;
        }

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     photon_view.RPC("UpdateHostPartList_RPC", RpcTarget.AllBuffered, bot_parts, bot_name);
        //     Debug.LogWarning(player1_bot.name);
        // }
        // else
        // {
        //     photon_view.RPC("UpdateClientPartList_RPC", RpcTarget.AllBuffered, bot_parts, bot_name);
        //     Debug.LogWarning(player2_bot.name);
        // }

        UpdateRobotStatsDisplay(robot);
    }

    [PunRPC]
    public void UpdateHostPartList_RPC(string bot_parts, string bot_name)
    {
        // player1_bot.name = bot_name;
        host_robot_name_plate_text.text = bot_name;
        host_player_parts.text = bot_parts;
    }

    [PunRPC]
    public void UpdateClientPartList_RPC(string bot_parts, string bot_name)
    {
        // player2_bot.name = bot_name;
        client_robot_name_plate_text.text = bot_name;
        client_player_parts.text = bot_parts;
    }

    void UpdateRobotData(Robot robot)
    {
        object current_frame;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_frame", out current_frame))
        {
            Hashtable updated_robot = new Hashtable();
            if (!robot.equipped_parts["frame"])
            {
                updated_robot.Add("robot_frame", "empty");
            }
            else
            {
                updated_robot.Add("robot_frame", robot.equipped_parts["frame"].name);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }

        object current_head;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_head", out current_head))
        {
            Hashtable updated_robot = new Hashtable();
            if (!robot.equipped_parts["head"])
            {
                updated_robot.Add("robot_head", "empty");
            }
            else
            {
                updated_robot.Add("robot_head", robot.equipped_parts["head"].name);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }

        object current_right_arm;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_r_arm", out current_right_arm))
        {
            Hashtable updated_robot = new Hashtable();
            if (!robot.equipped_parts["r_arm"])
            {
                updated_robot.Add("robot_r_arm", "empty");
            }
            else
            {
                updated_robot.Add("robot_r_arm", robot.equipped_parts["r_arm"].name);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }
        object current_left_arm;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_l_arm", out current_left_arm))
        {
            Hashtable updated_robot = new Hashtable();
            if (!robot.equipped_parts["l_arm"])
            {
                updated_robot.Add("robot_l_arm", "empty");
            }
            else
            {
                updated_robot.Add("robot_l_arm", robot.equipped_parts["l_arm"].name);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }

        object current_locomotion;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_locomotion", out current_locomotion))
        {
            Hashtable updated_robot = new Hashtable();
            if (!robot.equipped_parts["locomotion"])
            {
                updated_robot.Add("robot_locomotion", "empty");
            }
            else
            {
                updated_robot.Add("robot_locomotion", robot.equipped_parts["locomotion"].name);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }

        object is_currently_built;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("is_built", out is_currently_built))
        {
            Hashtable updated_robot = new Hashtable();
            updated_robot.Add("is_built", CheckRobotBuildComplete(robot));

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }

        UpdatePartsInfo(robot);
        // UpdateRobotStatsDisplay(robot);
    }

    void UpdateRobotName(Robot robot)
    {
        object current_name;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_name", out current_name))
        {
            Hashtable updated_robot = new Hashtable();
            updated_robot.Add("robot_name", robot.name);

            PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player target_player, Hashtable updatedHashtable)
    {
        if (target_player.IsMasterClient)
        {
            Debug.Log("Updating host robot");
            if (updatedHashtable.ContainsKey("robot_name"))
            {
                player1_bot.name = (string)target_player.CustomProperties["robot_name"];
            }
            if (updatedHashtable.ContainsKey("robot_frame"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_frame"])
                    {
                        player1_bot.frame = part as RobotFrame;
                    }
                    if ((string)target_player.CustomProperties["robot_frame"] == "empty")
                    {
                        player1_bot.frame = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_head"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_head"])
                    {
                        player1_bot.head = part as RobotPartHead;
                    }
                    if ((string)target_player.CustomProperties["robot_head"] == "empty")
                    {
                        player1_bot.head = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_r_arm"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_r_arm"])
                    {
                        player1_bot.right_arm = part as RobotPartArm;
                    }
                    if ((string)target_player.CustomProperties["robot_r_arm"] == "empty")
                    {
                        player1_bot.right_arm = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_l_arm"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_l_arm"])
                    {
                        player1_bot.left_arm = part as RobotPartArm;
                    }
                    if ((string)target_player.CustomProperties["robot_l_arm"] == "empty")
                    {
                        player1_bot.left_arm = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_locomotion"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_locomotion"])
                    {
                        player1_bot.locomotion = part as RobotPartLocomotion;
                    }
                    if ((string)target_player.CustomProperties["robot_locomotion"] == "empty")
                    {
                        player1_bot.locomotion = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("is_built"))
            {
                player1_bot.is_built = (bool)target_player.CustomProperties["is_built"];
            }

            UpdatePartsInfo(player1_bot);
        }
        else
        {
            Debug.Log("Updating client robot");
            if (updatedHashtable.ContainsKey("robot_name"))
            {
                player2_bot.name = (string)target_player.CustomProperties["robot_name"];
            }

            if (updatedHashtable.ContainsKey("robot_frame"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_frame"])
                    {
                        player2_bot.frame = part as RobotFrame;
                    }
                    if ((string)target_player.CustomProperties["robot_frame"] == "empty")
                    {
                        player2_bot.frame = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_head"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_head"])
                    {
                        player2_bot.head = part as RobotPartHead;
                    }
                    if ((string)target_player.CustomProperties["robot_head"] == "empty")
                    {
                        player2_bot.head = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_r_arm"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_r_arm"])
                    {
                        player2_bot.right_arm = part as RobotPartArm;
                    }
                    if ((string)target_player.CustomProperties["robot_r_arm"] == "empty")
                    {
                        player2_bot.right_arm = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_l_arm"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_l_arm"])
                    {
                        player2_bot.left_arm = part as RobotPartArm;
                    }
                    if ((string)target_player.CustomProperties["robot_l_arm"] == "empty")
                    {
                        player2_bot.left_arm = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("robot_locomotion"))
            {
                foreach (RobotPart part in part_registry)
                {
                    if (part.name == (string)target_player.CustomProperties["robot_locomotion"])
                    {
                        player2_bot.locomotion = part as RobotPartLocomotion;
                    }
                    if ((string)target_player.CustomProperties["robot_locomotion"] == "empty")
                    {
                        player2_bot.locomotion = null;
                    }
                }
            }
            if (updatedHashtable.ContainsKey("is_built"))
            {
                player2_bot.is_built = (bool)target_player.CustomProperties["is_built"];
            }

            UpdatePartsInfo(player2_bot);
        }
    }
}
