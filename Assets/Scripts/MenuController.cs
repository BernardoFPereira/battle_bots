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

public class MenuController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    BotBuildManager bot_build_manager;

    [SerializeField]
    BattlefieldManager battlefield_manager;

    [SerializeField]
    CombatManager combat_manager;

    [SerializeField]
    GameObject lobby, engagement, room;

    [SerializeField]
    TMP_InputField inputName;

    [SerializeField]
    TMP_Text host_username_display;

    [SerializeField]
    TMP_Text client_username_display;

    [SerializeField]
    GameObject connect_fail_popup;

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
        if (current_screen == Screen.ENGAGEMENT && Input.GetKeyDown(KeyCode.Return))
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

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_name", out object current_name))
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

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("battlefield_name", out object current_battlefield))
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

            Hashtable host_properties = PhotonNetwork.MasterClient.CustomProperties;
            bot_build_manager.OnPlayerPropertiesUpdate(PhotonNetwork.MasterClient, host_properties);

            bot_build_manager.UpdatePartsInfo(bot_build_manager.player1_bot);
            bot_build_manager.UpdatePartsInfo(bot_build_manager.player2_bot);

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("robot_name", out object current_name))
            {
                Hashtable updated_robot = new Hashtable();
                updated_robot.Add("robot_name", (string)current_name);

                PhotonNetwork.LocalPlayer.SetCustomProperties(updated_robot);
            }
        }

        SetPlayerProperties();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No room available!");
        connect_fail_popup.SetActive(true);
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

    public override void OnPlayerLeftRoom(Player other_player)
    {
        Debug.LogWarning("Oponente desconectado! Saindo da sala");

        if (bot_build_manager != null)
        {
            CombatManager combat_manager = FindObjectOfType<CombatManager>();
            if (combat_manager != null)
            {
                combat_manager.combat_running = false;
                combat_manager.StopAllCoroutines();
            }
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Saindo da sala. Voltando para a cena de engajamento!");

        ShowScreen(Screen.ENGAGEMENT);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Desconexão abrupta: " + cause.ToString());

        ShowScreen(Screen.ENGAGEMENT);
    }

    // public IEnumerator ReturnPing(float time)
    // {
    //     Debug.LogWarning("Server region: " + PhotonNetwork.CloudRegion);
    //     Debug.LogWarning("Ping: " + PhotonNetwork.GetPing());
    //     Debug.LogWarning("---------------------");
    //     yield return new WaitForSeconds(time);
    //     StartCoroutine("ReturnPing", 1f);
    // }

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

    [PunRPC]
    public void ExecuteRematchReset_RPC()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
            {"is_built", false},
            {"is_ready", false}
        });

        bot_build_manager.player1_bot.ClearRobotParts();
        bot_build_manager.player2_bot.ClearRobotParts();

        ReadyButton[] ready_buttons = FindObjectsOfType<ReadyButton>();
        foreach (ReadyButton button in ready_buttons)
        {
            button.UpdateReadyStatus_RPC(false);
        }

        bot_build_manager.ClearButtonTicks();

        combat_manager.combat_log.text = "";

        RematchButton rematch_button = FindObjectOfType<RematchButton>();
        if (rematch_button != null)
        {
            rematch_button.ClearRematchTicks_RPC();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            int random_idx = Random.Range(0, battlefield_manager.battlefield_list.Length - 1);
            string new_arena = battlefield_manager.battlefield_list[random_idx].map_name;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "battlefield_name", new_arena } });
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
