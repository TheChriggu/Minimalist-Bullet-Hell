using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class Player : MonoBehaviourPun, IOnEventCallback
{
    [SerializeField]
    private float speed = 1;                        //player movement speed
    private Vector3 shotDirection = Vector3.up;     //direction in which the player shoots
    public float shotFrequency = 0.5f;              //time between shooting bullets
    public GameObject bulletPrefab;                 //prefab of bullet the player shoots
    public GameObject helperPrefab;                 //prefab of the helper
    public Sprite normalSprite;                     //player sprite for normal state
    public Sprite invincibleSprite;                 //player sprite during invincibility
    private LifeCounter lifeCounter;                //reference to life counter

    //screen borders
    float minX;
    float minY;
    float maxX;
    float maxY;

    //main camera
    Camera cam;
    //player sprite
    SpriteRenderer sprite;

    bool isShooting;

    int bulletCount = 0;
    public int bulletsUntilHelper = 200;
    int bulletCountForHelper = 0;
    bool isInvincible = false;

    //listen to photon events
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    //stop listening to photon events
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        //setup camera & borders
        cam = Camera.main;

        var vertExtent = cam.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        minX = -horzExtent;
        maxX = horzExtent;
        minY = -vertExtent;
        maxY = vertExtent;

        //gather components
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = normalSprite;
        lifeCounter = GameObject.Find("Life Counter").GetComponent<LifeCounter>();

        //only controlled player is green, all others are blue
        if (!photonView.IsMine)
        {
            sprite.color = new Color(0, 0.5294117647f, 0.85882352941f);
        }

        //enable shooting
        InvokeRepeating("Fire", 0.0f, shotFrequency);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            //Set player position
            Vector3 position = transform.position;
            Vector3 direction = Vector3.zero;
            direction.x = Input.GetAxis("Horizontal");
            direction.y = Input.GetAxis("Vertical");
            direction.Normalize();
            position += direction * speed * Time.deltaTime;


            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            transform.position = position;

            //set player direction
            shotDirection.x = Input.GetAxis("FireHorizontal");
            shotDirection.y = Input.GetAxis("FireVertical");

            if (shotDirection.magnitude > 0.005)
            {
                shotDirection.Normalize();
                Quaternion quat = new Quaternion();
                quat.SetFromToRotation(transform.up, shotDirection);
                transform.rotation = quat * transform.rotation;
            }

            //player started shooting
            if (!isShooting && Input.GetAxis("Fire1") > 0.005)
            {
                var c = photonView.OwnerActorNr;
                object content = (object)c;
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerStartedShooting, content, options, SendOptions.SendReliable);
            }
            //player stopped shooting
            else if (isShooting && Input.GetAxis("Fire1") <= 0.005)
            {
                var c = photonView.OwnerActorNr;
                object content = (object)c;
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerStoppedShooting, content, options, SendOptions.SendReliable);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //If hit by enemy and not invincible, player can loose a life
            if (!isInvincible)
            {
                if (photonView.IsMine)
                {
                    GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.25f, 0.75f, 0.5f);

                    StartCoroutine("Flash");
                    lifeCounter.LooseALife();
                }
                else
                {
                    GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.25f, 0.25f, 0.25f);
                    StartCoroutine("Flash");
                }
            }
            //if player is invincible, he destroys the other enemy
            else
            {
                GameObject.Find("GamePad").GetComponent<GamePad>().SetPad(0.75f, 0.75f, 0.2f);
                other.GetComponent<Spawnable>().Despawn();
            }
        }
    }

    //Launches a bullet
    void Fire()
    {
        if (isShooting)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.GetComponent<PlayerBullet>().id = transform.position;
            bullet.GetComponent<PlayerBullet>().id.z = bulletCount;
            bulletCount++;
            bullet.GetComponent<SpriteRenderer>().color = sprite.color;

            //Track shot bullets to spawn a helper after a certain amount of bullets has been shot
            if (photonView.IsMine)
            {
                bulletCountForHelper++;
                if (bulletCountForHelper == bulletsUntilHelper)
                {
                    var c = photonView.OwnerActorNr;
                    object content = (object)c;
                    RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                    PhotonNetwork.RaiseEvent((byte)EventCodes.HelperSpawned, content, options, SendOptions.SendReliable);
                    StartHelper();
                }
            }



        }
    }

    //Turn the player white, the background red and shake the controller for a short time. Called when player has been hit by a bullet
    IEnumerator Flash()
    {
        cam.backgroundColor = new Color(0.5f, 0.0f, 0.04f);
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        cam.backgroundColor = new Color(0.09803922f, 0.1490196f, 0.1032343f);

        if (photonView.IsMine)
        {
            sprite.color = new Color(0, 1, 0.07073355f);
        }
        else
        {
            sprite.color = new Color(0, 0.5294117647f, 0.85882352941f);
        }
    }

    //listen to photon events
    public void OnEvent(EventData photonEvent)
    {
        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.PlayerStartedShooting:
                if ((int)photonEvent.CustomData == photonView.OwnerActorNr)
                {
                    Debug.Log("Start shooting actor " + photonView.OwnerActorNr);
                    isShooting = true;
                }
                break;

            case EventCodes.PlayerStoppedShooting:
                if ((int)photonEvent.CustomData == photonView.OwnerActorNr)
                {
                    Debug.Log("Stop shooting actor " + photonView.OwnerActorNr);
                    isShooting = false;
                }
                break;

            //End the game on player death
            case EventCodes.PlayerDead:
                GameObject.Find("GamePad").GetComponent<GamePad>().StopPad();
                SceneManager.LoadScene("GameOver");
                break;


            case EventCodes.HelperSpawned:
                if ((int)photonEvent.CustomData == photonView.OwnerActorNr)
                {
                    StartHelper();
                }
                break;

            case EventCodes.HelperDespawned:
                if ((int)photonEvent.CustomData == photonView.OwnerActorNr)
                {
                    //restart counting the bullets until the next helper spawn
                    bulletCountForHelper = 0;
                }
                break;

            case EventCodes.PlayerInvincibleStart:
                if ((int)photonEvent.CustomData == photonView.OwnerActorNr)
                {
                    StartCoroutine("InvincibilityTimer");
                }
                break;
        }
    }

    //creates a helper
    void StartHelper()
    {
        var helper = Instantiate(helperPrefab);
        helper.GetComponent<Helper>().player = this.gameObject;
    }

    //Called by pickup. Start player invincibility
    public void StartInvincibility()
    {
        var c = photonView.OwnerActorNr;
        object content = (object)c;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerInvincibleStart, content, options, SendOptions.SendReliable);


        StartCoroutine("InvincibilityTimer");
    }

    //Turns player white and invincible for 10 seconds
    IEnumerator InvincibilityTimer()
    {
        sprite.sprite = invincibleSprite;
        isInvincible = true;
        sprite.color = Color.white;
        yield return new WaitForSeconds(10.0f);
        sprite.sprite = normalSprite;
        isInvincible = false;

        if (photonView.IsMine)
        {
            sprite.color = new Color(0, 1, 0.07073355f);
        }
        else
        {
            sprite.color = new Color(0, 0.5294117647f, 0.85882352941f);
        }
    }
}
