using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LeaveRoom : MonoBehaviour
{

    void Start()
    {
        PhotonNetwork.LeaveRoom();
    }
}
