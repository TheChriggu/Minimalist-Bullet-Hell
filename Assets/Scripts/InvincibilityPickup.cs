using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityPickup : Spawnable
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if(player != null)
        {
            player.StartInvincibility();
            Despawn();
        }
    }
}
