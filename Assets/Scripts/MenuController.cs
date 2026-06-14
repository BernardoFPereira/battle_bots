using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum Screen
{
    ENGAGEMENT,
    LOBBY,
    ROOM,
}

public class NewBehaviourScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    BotBuildManager bot_build_manager;

    [SerializeField]
    BattlefieldManager battlefield_manager;

    [SerializeField]
    GameObject lobby, engagement, room;

    [SerializeField]
    TMP_InputField inputName;

    [SerializeField]
    TMP_Text host_username_display;

    [SerializeField]
    TMP_Text client_username_display;

    Screen current_screen;

    public void Start()
    {
        ShowScreen(Screen.ENGAGEMENT);
    }

    public void Update()
    {
        HandleEngagementInput();
    }

    void HandleEngagementInput()
    {
        if (current_screen == Screen.ENGAGEMENT && Input.anyKeyDown)
        {
            ConnectToPhoton();
        }
    }

    void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        Debug.Log("Conectado!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao Master!");
        // StartCoroutine("ReturnPing", 1f);
        EnterLobby();
    }

    public void EnterLobby()
    {
        Debug.Log("Tentando entrar em um lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entrei num Lobby!");
        ShowScreen(Screen.LOBBY);
    }

    public void EnterRoom()
    {
        Debug.Log("Tentando entrar em uma Sala!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("WELCOME!");
        ShowScreen(Screen.ROOM);
        if (PhotonNetwork.IsMasterClient)
        {
            host_username_display.text = "<color=#6495ED>" + PhotonNetwork.NickName + "</color>";
            bot_build_manager.player1_bot.InitializeRobot();
            bot_build_manager.UpdatePartsInfo(bot_build_manager.player1_bot);
            bot_build_manager.UpdatePartsInfo(bot_build_manager.player2_bot);

            object current_name;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_name", out current_name))
            {
                Hashtable updated_robot = new Hashtable();
                updated_robot.Add("robot_name", (string)current_name);

                PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
            }
        }
        else
        {
            host_username_display.text = "<color=#6495ED>" + PhotonNetwork.MasterClient.NickName + "</color>";
            client_username_display.text = "<color=yellow>" + PhotonNetwork.NickName + "</color>";
            bot_build_manager.player2_bot.InitializeRobot();
            bot_build_manager.UpdatePartsInfo(bot_build_manager.player1_bot);
            bot_build_manager.UpdatePartsInfo(bot_build_manager.player2_bot);

            object current_name;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_name", out current_name))
            {
                Hashtable updated_robot = new Hashtable();
                updated_robot.Add("robot_name", (string)current_name);

                PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
            }

            object current_battlefield;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("battlefield_name", out current_battlefield))
            {
                foreach (Battlefield battlefield in battlefield_manager.battlefield_list)
                {
                    if ((string)current_battlefield == battlefield.map_name)
                    {
                        battlefield_manager.selected_arena = battlefield;
                        battlefield_manager.UpdateAreaScan(battlefield);
                    }
                }
            }
        }

        SetPlayerProperties();
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No room available!");
        // TODO: Avisar usuario com feedback na tela (janela de erro talvez)
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log("Player " + player.NickName + " entered the room!");

        if (PhotonNetwork.IsMasterClient)
        {
            host_username_display.text = "<color=#6495ED>" + PhotonNetwork.NickName + "</color>";
            client_username_display.text = "<color=yellow>" + player.NickName + "</color>";
        }
        else
        {
            host_username_display.text = "<color=#6495ED>" + PhotonNetwork.MasterClient.NickName + "</color>";
            client_username_display.text = "<color=yellow>" + PhotonNetwork.NickName + "</color>";
        }
    }

    public IEnumerator ReturnPing(float time)
    {
        Debug.LogWarning("Server region: " + PhotonNetwork.CloudRegion);
        Debug.LogWarning("Ping: " + PhotonNetwork.GetPing());
        Debug.LogWarning("---------------------");
        yield return new WaitForSeconds(time);
        StartCoroutine("ReturnPing", 1f);
    }

    void ShowScreen(Screen screen)
    {
        HideAllScreens();

        switch (screen)
        {
            case Screen.ENGAGEMENT:
                engagement.SetActive(true);
                current_screen = screen;
                break;
            case Screen.LOBBY:
                lobby.SetActive(true);
                current_screen = screen;
                break;
            case Screen.ROOM:
                room.SetActive(true);
                current_screen = screen;
                break;
        }
    }

    void HideAllScreens()
    {
        lobby.SetActive(false);
        engagement.SetActive(false);
        room.SetActive(false);
    }

    public void ConnectUser()
    {
        SaveUsername();
        EnterRoom();
    }

    void SaveUsername()
    {
        string temp_name = inputName.text;
        if (temp_name == "")
        {
            temp_name = "Player_" + Random.Range(0, 99);
        }

        PhotonNetwork.NickName = temp_name;
    }

    public void CreateRoom()
    {
        SaveUsername();
        string room_name = "Room_" + Random.Range(10, 999);
        PhotonNetwork.CreateRoom(room_name);
    }

    public override void OnCreatedRoom()
    {
        SetRoomProperties();
        battlefield_manager.InitializeBattlefield();
    }

    void SetPlayerProperties()
    {
        Hashtable temp_properties = new Hashtable();

        temp_properties.Add("username", PhotonNetwork.NickName);
        temp_properties.Add("ID", PhotonNetwork.LocalPlayer.UserId);
        temp_properties.Add("robot_frame", "empty");
        temp_properties.Add("robot_head", "empty");
        temp_properties.Add("robot_r_arm", "empty");
        temp_properties.Add("robot_l_arm", "empty");
        temp_properties.Add("robot_locomotion", "empty");
        temp_properties.Add("is_built", false);

        PhotonNetwork.SetPlayerCustomProperties(temp_properties);
    }

    void SetRoomProperties()
    {
        Hashtable temp_room_properties = new Hashtable();

        temp_room_properties.Add("battlefield_name", "");
        temp_room_properties.Add("battlefield_environ", "");
        temp_room_properties.Add("battlefield_size", "");
        temp_room_properties.Add("battlefield_visibility", "");
        temp_room_properties.Add("battlefield_terrain", "");

        PhotonNetwork.CurrentRoom.SetCustomProperties(temp_room_properties);
    }
}
