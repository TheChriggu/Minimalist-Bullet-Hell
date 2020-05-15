using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class PlayerBullet : Bullet
{
    //Player bullet destroys self on encounter with enemies and supports
    internal override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "Support")
        {
            object content = (object)id;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
            PhotonNetwork.RaiseEvent((byte)EventCodes.BulletDestroyed, content, options, SendOptions.SendReliable);
            Destroy(gameObject);
        }
    }
}
