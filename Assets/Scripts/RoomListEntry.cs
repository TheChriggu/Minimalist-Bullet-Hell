using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class RoomListEntry : MonoBehaviour
{
    public Text roomName;
    public Text players;
    public Text lives;
    public Button joinButton;

    void Start()
    {
        joinButton.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        GameObject.FindObjectOfType<Lobby>().JoinRoom(roomName.text);
    }

    public void DisplayRoomInfo(RoomInfo info)
    {
        if (info.PlayerCount != 0)
        {
            roomName.text = info.Name;
            players.text = info.PlayerCount.ToString() + "/" + info.MaxPlayers.ToString();
            lives.text = ((int)info.CustomProperties["l"]).ToString();
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
