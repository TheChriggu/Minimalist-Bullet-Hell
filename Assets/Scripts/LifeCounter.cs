using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LifeCounter : MonoBehaviourPun, IOnEventCallback
{
    public GameObject lifePrefab;


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        int lives = (int)PhotonNetwork.CurrentRoom.CustomProperties["l"];
        for(int i=0; i<lives; i++)
        {
            var instance = Instantiate(lifePrefab);
            instance.transform.parent = transform;
        }
    }

    public void LooseALife()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent((byte)EventCodes.LooseALife, 0, options, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.LooseALife:
                if(transform.childCount > 1)
                {
                    Destroy(transform.GetChild(transform.childCount - 1).gameObject);
                }


                else if (transform.childCount <= 1)
                {
                    GameObject.Find("GamePad").GetComponent<GamePad>().StopPad();

                    RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerDead, 0, options, SendOptions.SendReliable);
                }
                break;

        }
    }
}
