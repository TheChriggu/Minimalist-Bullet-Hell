using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PlayAgainButton : MonoBehaviour
{
    public void OnButtonDown()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("Lobby");
    }
}
