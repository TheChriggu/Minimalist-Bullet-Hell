using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Spawnable : MonoBehaviour,  IOnEventCallback
{
    public Vector3 targetLocation;
    public float movementSpeed = 5.0f;

    internal SpriteRenderer sprite;
    bool hasArrived = false;

    //set as callback target
    protected virtual void Awake() 
    {
        PhotonNetwork.AddCallbackTarget(this);
        sprite = GetComponent<SpriteRenderer>();
    }

    //remove from callback targets
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        //spawnable should move towards target, and call the OnArrival function once it has arrived
        if((transform.position - targetLocation).magnitude > 0.5)
        {
            MoveTowardsTarget();
        }
        else if(!hasArrived)
        {
            OnArrival();
            hasArrived = true;
        }
    }

    void MoveTowardsTarget()
    {
        transform.Translate((targetLocation-transform.position).normalized * movementSpeed * Time.deltaTime, Space.World);
    }


    //Function called, once target location is reached
    internal virtual void OnArrival()
    {}

    //react to photon events
    public virtual void OnEvent(EventData photonEvent)
    {

        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.SpawnableDespawned: //Caused, when spawnable has despawned on any other client
                var vec = (Vector2)photonEvent.CustomData;
                if(targetLocation.x == vec.x && targetLocation.y == vec.y) 
                {
                    Despawn();
                }
                break;
        }
    }

    //Informs all other spawnables that this has been despawned, then destroys itself
    public void Despawn() 
    {
        Vector2 data = new Vector2(targetLocation.x, targetLocation.y);
        object content = (object)data;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
        PhotonNetwork.RaiseEvent((byte)EventCodes.SpawnableDespawned, content, options, SendOptions.SendReliable);

        Destroy(gameObject);
    }
}
