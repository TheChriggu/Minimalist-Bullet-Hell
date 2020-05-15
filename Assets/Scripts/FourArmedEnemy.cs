using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourArmedEnemy : Enemy
{
    public float angleStep = 5.0f;  //how many degrees this enemy moves before the next shot

    //firing pattern
    override internal void Fire()
    {
        //first rotate this enemy
        transform.Rotate(new Vector3(0, 0, 1), angleStep, Space.World);

        //launch 4 bullets with 90 degrees between them
        base.LaunchBullet(transform.position, transform.eulerAngles);
        base.LaunchBullet(transform.position, transform.eulerAngles + new Vector3(0, 0, 90));
        base.LaunchBullet(transform.position, transform.eulerAngles + new Vector3(0, 0, 180));
        base.LaunchBullet(transform.position, transform.eulerAngles + new Vector3(0, 0, 270));
    }
}
