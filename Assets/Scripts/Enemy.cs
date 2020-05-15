using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Enemy : Spawnable
{

    [SerializeField]
    internal GameObject bulletPrefab;   //bullet prefab
    public int lives = 10;              //how many hits until this enemy is considered dead
    public int scorePoints = 50;        //points upon death of this enemy
    public float shotFrequency = 0.3f; //frequency at which this enemy shoots
    public float bulletScale = 1.0f;    //size of the bullet
    int bulletCount = 0;                //count of bullets this enemy has shot
    Color color;                        //color of this enemy. only used for flashing


    protected override void Awake()
    {
        base.Awake();
        //save the sprite color
        color = GetComponent<SpriteRenderer>().color;
    }

    //Reaction to being shot by player
    internal virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.0f, 0.75f, 0.05f);

            //increase size slightly
            transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);

            StartCoroutine("Flash");

            //loose a life
            lives -= 1;
            if (lives <= 0)
            {
                Die();
            }
        }
    }

    //Override for special behaviour for death
    internal virtual void Die()
    {
        Despawn();
    }

    //When target location is reached, start shooting
    internal override void OnArrival()
    {
        InvokeRepeating("Fire", 0.001f, shotFrequency);
    }

    //code to override for firing pattern
    virtual internal void Fire()
    { }

    //launch a bullet
    virtual internal void LaunchBullet(Vector3 position, Vector3 rotation)
    {
        //Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = position;
        bullet.transform.eulerAngles = rotation;
        bullet.transform.localScale *= bulletScale;

        //set bullet Id as combination of enemy position and bullet count
        bullet.GetComponent<Bullet>().id = targetLocation;
        bullet.GetComponent<Bullet>().id.z = bulletCount;
        bulletCount++;
    }


    //turn the sprite white for a short moment
    internal IEnumerator Flash()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        sprite.color = color;
    }

    //Add score to scoreboard
    void OnDestroy()
    {
        var score = GameObject.FindObjectOfType<Score>();
        if(score)
        {
            score.AddPoints(scorePoints);
        }
    }
}
