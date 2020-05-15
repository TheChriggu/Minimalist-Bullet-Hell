using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingEnemy : Enemy
{

    [SerializeField]
    private Player[] players;   //reference to all players, so this enemy can always aim at the closest one

    void Start()
    {
        //save all players
        players = GameObject.FindObjectsOfType<Player>();
    }

    override internal void Fire()
    {
        //Aim at closest player
        GameObject player = players[0].gameObject;
        float distance = (player.transform.position - transform.position).magnitude;
        Vector3 direction = player.transform.position - transform.position;

        foreach (var p in players)
        {
            Vector3 newDirection = p.gameObject.transform.position - transform.position;
            if(newDirection.magnitude < distance)
            {
                direction = newDirection;
                distance = newDirection.magnitude;
                player = p.gameObject;
            }
        }

        
        Quaternion quat = new Quaternion();
        quat.SetFromToRotation(transform.up, direction);
        transform.rotation = quat * transform.rotation;
        base.LaunchBullet(transform.position, transform.eulerAngles);
    }
}
