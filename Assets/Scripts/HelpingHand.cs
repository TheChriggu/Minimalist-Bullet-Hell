using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class HelpingHand : Enemy
{
    public int explosionBulletCount = 72;       //amount of bullets launched during explosion
    bool isExploded = false;                    //tracker to ensure that Explode function is only called once

    //This enemy explodes, rather than dying the normal way
    override internal void Die()
    {
        Explode();
    }

    //spawns bullets around this enemy
    private void Explode()
    {
        if (!isExploded)
        {
            isExploded = true;

            //send explosion message to all listeners
            Vector2 data = new Vector2(targetLocation.x, targetLocation.y);
            object content = (object)data;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)EventCodes.EnemyExploded, content, options, SendOptions.SendReliable);

            //shake game pad a lot.
            GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.75f, 0.25f, 1.5f);

            float angleStep = 360 / explosionBulletCount;
            for (int i = 0; i < explosionBulletCount; i++)
            {
                //rotate
                transform.Rotate(new Vector3(0, 0, 1), angleStep, Space.World);

                //launch bullets slightly off center, to ease collision calculations
                base.LaunchBullet(transform.position + transform.up * 0.5f, transform.eulerAngles);
            }
            
            Despawn();
        }

    }


    //respond to photon events
    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);

        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.EnemyExploded:
                var vec2 = (Vector2)photonEvent.CustomData;
                if (targetLocation.x == vec2.x && targetLocation.y == vec2.y)
                {
                    Explode();
                }
                break;
        }
    }
}
