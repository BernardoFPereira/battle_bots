using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Robot : MonoBehaviour
{
    public new string name;
    public RobotFrame frame;
    public RobotPartHead head;
    public RobotPartArm right_arm;
    public RobotPartArm left_arm;
    public RobotPartLocomotion locomotion;

    public Dictionary<string, RobotPart> equipped_parts;

    readonly string[] name_list = { "Haulberk", "Gypsy", "Needle", "Bulwark", "Chromar", "Shorty", "Vermin", "Mintaka" };

    public bool is_built = false;
    public bool is_ready = false;

    // int total_integrity = 0;
    // int current_integrity = 0;

    PhotonView photon_view;

    void Awake()
    {
        photon_view = GetComponent<PhotonView>();

        equipped_parts = new Dictionary<string, RobotPart>();
        equipped_parts.Add("frame", null);
        equipped_parts.Add("head", null);
        equipped_parts.Add("r_arm", null);
        equipped_parts.Add("l_arm", null);
        equipped_parts.Add("locomotion", null);

        name = "NOT JOINED";
    }

    // private void Start()
    // {
    //     photon_view = GetComponent<PhotonView>();
    // }

    public void InitializeRobot()
    {
        int rnd_idx = Random.Range(0, name_list.Length - 1);
        name = name_list[rnd_idx];

        Hashtable updated_robot = new Hashtable();
        updated_robot.Add("robot_name", name);
        PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
    }

    void PrintDebugData()
    {
        Debug.Log("--- Robot Info ---");
        Debug.Log("_designation: " + name);
        Debug.Log("Frame:" + frame.name);
        Debug.Log("_armor:" + frame.armor);
        Debug.Log("_integrity:" + frame.integrity);
        Debug.Log("Arms: " + right_arm.name + " // " + left_arm.name);
        Debug.Log("Locomotion: " + locomotion.name);
    }
}
