using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviourPunCallbacks
{
    public RoomList listDisplay;
    public Text newRoomName;
    public Text newRoomPlayers;
    public Text newRoomLives;

    void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public override void OnConnectedToMaster()
    {
       PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        listDisplay.SetRooms(roomList);
    }

    public void OnCreateRoom()
    {
        int maxPlayers;
        int lives;
        var customPropertiesForLobby = new string[1];
        RoomOptions options;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();

        
        //set lives at start of game as custom property
        if(int.TryParse(newRoomLives.text, out lives))
        {
            props.Add("l",lives); 
            customPropertiesForLobby[0] = "l";
        }
        else
        {
            props.Add("l",3); 
            customPropertiesForLobby[0] = "l";
        }

        if(int.TryParse(newRoomPlayers.text, out maxPlayers))
        {
            options = new RoomOptions
            {
                MaxPlayers = (byte) maxPlayers,
                CustomRoomProperties = props,
                CustomRoomPropertiesForLobby = customPropertiesForLobby
            };
        }
        else
        {
            options = new RoomOptions
            {
                MaxPlayers = 2,
                CustomRoomProperties = props,
                CustomRoomPropertiesForLobby = customPropertiesForLobby
            };
        }

        PhotonNetwork.CreateRoom(newRoomName.text, options);
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            LoadWaitingRoom();
        }
    }

    void LoadWaitingRoom()
    {
        SceneManager.LoadScene("WaitingRoom");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        SceneManager.LoadScene("WaitingRoom");
    }
}
