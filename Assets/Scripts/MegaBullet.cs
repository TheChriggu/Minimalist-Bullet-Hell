using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBullet : Bullet
{
    //MegaBullet destroys all objects with enemy tag it encounters, and doesn't die
    internal override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.Despawn();
            }
        }
    }
}
