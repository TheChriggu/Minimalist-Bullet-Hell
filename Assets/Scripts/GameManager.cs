using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public enum SpawnableTypes
{
    Aiming,
    FourArms,
    Exploding,
    Helping,
    Invincibility
}

public class SpawnData
{
    public SpawnableTypes type;
    public float target_x;
    public float target_y;
    public int startingPosition;
}

public enum EventCodes
{
    None = 0,
    PlayerStartedShooting,
    PlayerStoppedShooting,
    SpawnableSpawned,
    SpawnableDespawned,
    EnemyExploded,
    PlayerShot,
    PlayerDead,
    BulletDestroyed,
    HelperSpawned,
    HelperDespawned,
    HelperShot,
    PlayerInvincibleStart,
    LooseALife
}
public class GameManager : MonoBehaviourPunCallbacks
{
    private void OnLevelWasLoaded()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
