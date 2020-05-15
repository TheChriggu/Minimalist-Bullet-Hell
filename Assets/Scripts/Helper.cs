using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class Helper : MonoBehaviourPun, IOnEventCallback
{
    public float rotationSpeed = 50.0f;
    public GameObject player;
    public GameObject triangle;
    public SpriteRenderer sprite;

    public int lives = 5;
    public GameObject bulletPrefab;
    int bulletCount = 0;
    bool isShot = false;
    bool isShooting = false;

    GameObject target;

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
        if (player.GetComponent<PhotonView>().IsMine)
        {
            sprite.color = new Color(0, 1, 0.07073355f);
        }
        else
        {
            sprite.color = new Color(0, 0.5294117647f, 0.85882352941f);
        }
        
        InvokeRepeating("LaunchBullet", 0.0f, 0.4f);
    }

    void Update()
    {
        transform.position = player.transform.position;

        if (target == null)
        {
            isShooting = false;
            FindTarget();
        }
        else
        {
            isShooting = true;
            AimAtTarget();
        }
    }

    private void AimAtTarget()
    {
        Vector3 direction = target.transform.position - transform.position;
        Quaternion quat = new Quaternion();
        quat.SetFromToRotation(transform.up, direction);
        transform.rotation = quat * transform.rotation;
    }

    private void FindTarget()
    {
        var enemies = GameObject.FindObjectsOfType<Enemy>();

        Vector3 direction = new Vector3(100,100,100);
        float distance = 1000;
        foreach (var enemy in enemies)
        {
            direction = enemy.gameObject.transform.position - transform.position;
            if(direction.magnitude < distance)
            {
                target = enemy.gameObject;
            }
        }
    }

    void LaunchBullet()
    {
        if (isShooting)
        {
            //GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.5f, 0.0f, 0.01f);
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = triangle.transform.position;
            bullet.transform.rotation = triangle.transform.rotation;
            bullet.GetComponent<PlayerBullet>().id = triangle.transform.position;
            bullet.GetComponent<PlayerBullet>().id.z = bulletCount;
            bullet.transform.localScale /= 1.5f;
            bulletCount++;

            bullet.GetComponent<SpriteRenderer>().color = sprite.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            isShot = true;
            StartCoroutine("isShotTimer");

            var c = player.GetComponent<PhotonView>().OwnerActorNr;

            Debug.Log("helper " + c.ToString() + " shot, sending message...");

            object content = (object)c;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)EventCodes.HelperShot, content, options, SendOptions.SendReliable);
        }
    }

    IEnumerator isShotTimer()
    {
        yield return new WaitForSeconds(0.2f);
        isShot = false;
    }

    public void OnEvent(EventData photonEvent)
    {
        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.None:
                break;

            case EventCodes.HelperShot:
                if (isShot && (int)photonEvent.CustomData == player.GetComponent<PhotonView>().OwnerActorNr)
                {
                    isShot = false;

                    StartCoroutine("WhitenessTimer");
                    lives--;
                    if (lives <= 0)
                    {

                        var c = player.GetComponent<PhotonView>().OwnerActorNr;
                        object content = (object)c;
                        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEvent((byte)EventCodes.HelperDespawned, content, options, SendOptions.SendReliable);
                    }
                }
                Debug.Log("received shot message from: " + (int)photonEvent.CustomData + ". Own number is: " + player.GetComponent<PhotonView>().OwnerActorNr);
                break;
            case EventCodes.HelperDespawned:
                if ((int)photonEvent.CustomData == player.GetComponent<PhotonView>().OwnerActorNr)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    IEnumerator WhitenessTimer()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            sprite.color = new Color(0, 1, 0.07073355f);
        }
        else
        {
            sprite.color = new Color(0, 0.5294117647f, 0.85882352941f);
        }
    }
}
