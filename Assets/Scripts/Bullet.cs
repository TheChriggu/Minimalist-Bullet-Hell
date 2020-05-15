using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Bullet :  MonoBehaviourPun, IOnEventCallback
{
    [SerializeField]
    private float speed = 5.0f;     //speed at which this bullet moves
    public Vector3 id;              //id this bullet gets recognized at. Consists of the location of the shooter and its bullet count

    //subscribe to photon events
    internal void Awake() 
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    //remove from photon events
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    //move the bullet straight forward into the direction its pointing
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    //destroy the bullet after it leaves the screen
     void OnBecameInvisible() {
         Destroy(gameObject);
     }

    //bullet self destructs on collision with anything with tag player
    internal virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            object content = (object)id;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
            PhotonNetwork.RaiseEvent((byte)EventCodes.BulletDestroyed, content, options, SendOptions.SendReliable);
            Destroy(gameObject);
        }

    }

    //react to photon events
    public virtual void OnEvent(EventData photonEvent)
    {

        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.BulletDestroyed:
                var destroyedBullet = (Vector3)photonEvent.CustomData;
                if(id == destroyedBullet)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
}
