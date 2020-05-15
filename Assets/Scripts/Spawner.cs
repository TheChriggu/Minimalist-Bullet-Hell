using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Spawner : MonoBehaviourPun, IOnEventCallback
{
    //Prefabs for all spawnables
    public GameObject aimingEnemyPrefab;
    public GameObject fourArmsPrefab;
    public GameObject explodingEnemyPrefab;
    public GameObject explodingHelperPrefab;
    public GameObject invincibilityPickupPrefab;

    //screen space, all spawnables should end up between min and max
    float minX;
    float minY;
    float maxX;
    float maxY;

    //time in seconds for the next spawnable to spawn
    float spawnRate = 3.0f;
    //true, if the next spawn should be a friendly
    bool spawnFriendly = false;

    //time until next spawn
    float friendlyTimer = 1;

    //Add self as callback target to receive events
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    //remove self from callbacks when disabled
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        //calculate screen size, to determine area in which enemies can end up
        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        minX = -horzExtent + 1;
        maxX = horzExtent - 1;
        minY = -vertExtent + 1;
        maxY = vertExtent - 1;

        //start spawner
        InvokeRepeating("SpawnRandomEnemy", 0.0f, spawnRate);
        StartCoroutine("FriendlySpawnTimer");
    }

    //MasterClient spawns a new enemy at spwan rate
    void SpawnRandomEnemy()
    {
        //only master client can spawn enemies
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        //create SpawnData
        var data = new SpawnData();
        data.type = GetSpawnableType();
        data.target_x = Random.Range(minX, maxX);
        data.target_y = Random.Range(minY, maxY);
        int rand = Random.Range(0, transform.childCount); //this game objects children are the start positions for the spawnable
        data.startingPosition = rand;

        //pack SpawnData into a quaternion and send it as an event
        Quaternion quat;
        quat.x = (float) data.type;
        quat.y = (float) data.target_x;
        quat.z = (float) data.target_y;
        quat.w = (float) data.startingPosition;
        object content = (object)quat;
        
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((byte)EventCodes.SpawnableSpawned, content, options, SendOptions.SendReliable);
    }

    //Returns the type of the spawnable which should be spawned
    SpawnableTypes GetSpawnableType()
    {
        int rand = Random.Range(0, 100);

        //spawn a friendly if it is time, and restart the friendly timer
        if (spawnFriendly)
        {
            spawnFriendly = false;
            StartCoroutine("FriendlySpawnTimer");
            if(rand%2 == 0)
            {
                return SpawnableTypes.Invincibility;
            }
            else
            {
                return SpawnableTypes.Helping;
            }

        }

        //otherwise spawn an enemy
        switch (rand % 3)
        {
            case (0):
                return SpawnableTypes.Aiming;

            case (1):
                return SpawnableTypes.FourArms;

            case (2):
                return SpawnableTypes.Exploding;

            default:
                return SpawnableTypes.FourArms;
        }
    }

    //Friendly spawn timer increases by 5 seconds after every use
    IEnumerator FriendlySpawnTimer()
    {
        yield return new WaitForSeconds(friendlyTimer);
        spawnFriendly = true;
        friendlyTimer += 5.0f;
    }

    //spawns an enemy based on SpawnData
    void SpawnEnemy(SpawnData data)
    {
        //instantiate Spawnable
        GameObject spawnable = Instantiate(GetPrefabFromType(data.type));

        //set Spawnables starting point
        Vector3 startPosition = transform.GetChild(data.startingPosition).transform.position;
        spawnable.transform.position = startPosition;

        //set Spawnables target location
        Vector3 position = Vector3.zero;
        position.x = data.target_x;
        position.y = data.target_y; 
        spawnable.GetComponent<Spawnable>().targetLocation = position;


    }

    //Returns the prefab corresponding to the type of spawnable
    GameObject GetPrefabFromType(SpawnableTypes type)
    {
        switch (type)
        {
            case SpawnableTypes.Aiming:
                return aimingEnemyPrefab;

            case SpawnableTypes.Exploding:
                return explodingEnemyPrefab;

            case SpawnableTypes.FourArms:
                return fourArmsPrefab;

            case SpawnableTypes.Helping:
                return explodingHelperPrefab;
            case SpawnableTypes.Invincibility:
                return invincibilityPickupPrefab;

            default:
                return fourArmsPrefab;
        }
    }

    //Receive & Process photon events
    public void OnEvent(EventData photonEvent)
    {

        EventCodes code = (EventCodes)photonEvent.Code;

        switch (code)
        {
            case EventCodes.SpawnableSpawned:
                var quat = (Quaternion)photonEvent.CustomData;

                SpawnData data = new SpawnData();
                data.type = (SpawnableTypes) quat.x;
                data.target_x = (float) quat.y;
                data.target_y = (float) quat.z;
                data.startingPosition = (int) quat.w;
                SpawnEnemy(data);
                break;
        }
    }
}
