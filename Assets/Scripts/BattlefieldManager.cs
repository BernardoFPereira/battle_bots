using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BattlefieldManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Battlefield[] battlefield_list;

    [SerializeField]
    TMP_Text environ_text;

    [SerializeField]
    TMP_Text size_text;

    [SerializeField]
    TMP_Text visibility_text;

    [SerializeField]
    TMP_Text terrain_text;

    [SerializeField]
    Image map_image_display;

    public Battlefield selected_arena;

    public void InitializeBattlefield()
    {
        int rnd_idx = Random.Range(0, battlefield_list.Length - 1);
        selected_arena = battlefield_list[rnd_idx];

        Hashtable updated_properties = new Hashtable();

        updated_properties.Add("battlefield_name", selected_arena.map_name);
        updated_properties.Add("battlefield_environ", selected_arena.environ);
        updated_properties.Add("battlefield_size", selected_arena.map_size);
        updated_properties.Add("battlefield_visibility", selected_arena.visibility);
        updated_properties.Add("battlefield_terrain", selected_arena.terrain);

        PhotonNetwork.CurrentRoom.SetCustomProperties(updated_properties);
    }

    public void UpdateAreaScan(Battlefield battlefield)
    {
        environ_text.text = battlefield.environ.ToString();
        size_text.text = battlefield.map_size.ToString();
        visibility_text.text = battlefield.visibility.ToString();
        terrain_text.text = battlefield.terrain.ToString();
        map_image_display.sprite = battlefield.map_sprite;
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProperties)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            foreach (Battlefield battlefield in battlefield_list)
            {
                if ((string)changedProperties["battlefield_name"] == battlefield.map_name)
                {
                    UpdateAreaScan(battlefield);
                }
            }
        }
    }
}
