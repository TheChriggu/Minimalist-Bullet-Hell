using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LogIn : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";
    public Text userName;
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnLogin()
    {
        string name = userName.text;
        
        if(name != "")
        {
            PhotonNetwork.NickName = name;
            Connect();
        }
    }

    void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
