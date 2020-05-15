using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.Disconnect();     
        Destroy(GameObject.Find("Persistent"));
    }

}
