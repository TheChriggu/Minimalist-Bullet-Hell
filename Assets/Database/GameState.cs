using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;


public static class UnityVector4Extender
{
    public static SerializableVector4 ToSerializableVec4(this Vector2 v)
    {
        return new SerializableVector4(){x = v.x, y = v.y};
    }
    public static SerializableVector4 ToSerializableVec4(this Vector3 v)
    {
        return new SerializableVector4(){x = v.x, y = v.y, z = v.z};
    }
    public static SerializableVector4 ToSerializableVec4(this Quaternion v)
    {
        return new SerializableVector4(){x = v.x, y = v.y, z = v.z, w = v.w};
    }

    public static Vector2 ToVector2(this SerializableVector4 v)
    {
        return new Vector2(v.x, v.y);
    }
    public static Vector3 ToVector3(this SerializableVector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
    public static Quaternion ToVector4(this SerializableVector4 v)
    {
        return new Quaternion(v.x, v.y,v.z,v.w);
    }
}

public class SerializableVector4
{
    public float x,y,z,w;
}
public class SaveData
{
    public enum Type
    {
        PlayerBullet,
        MegaBullet,
        EnemyBullet,
        FourArmsEnemy,
        AimingEnemy,
        ExplodingEnemy,
        HelpingEnemy
    }

    public Type type;
    public float scale;
    public SerializableVector4 position;
    public SerializableVector4 rotation;
    public int lifes;
    public SerializableVector4 targetPosition;
}
public interface ISaveable
{
    SaveData GetSaveData();
}

public class SaveListContainer
{
    [BsonId]
    public ObjectId id;
    public List<SaveData> saveDatas;
}

public class GameState : MonoBehaviour
{
    /*
    Database database;
    List<SaveData> saveDatas;

    [BsonId]
    ObjectId id;

    public GameObject EnemyBulletPrefab;
    public GameObject MegaBulletPrefab;
    public GameObject PlayerBulletPrefab;
    public GameObject AimingEnemyPrefab;
    public GameObject FourArmedEnemyPrefab;
    public GameObject ExplodingEnemyPrefab;
    public GameObject HelpingHandPrefab;

    // Start is called before the first frame update
    void Start()
    {
        database = new Database();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0"))
        {
            Save();
        }

        if (Input.GetKeyDown("joystick button 2"))
        {
            Debug.Log("btn 2");
            Load();
        }
    }

    public void Save()
    {
        saveDatas = new List<SaveData>();

        AddDatasToList(FindObjectsOfType<AimingEnemy>());
        AddDatasToList(FindObjectsOfType<ExplodingEnemy>());
        AddDatasToList(FindObjectsOfType<FourArmedEnemy>());
        AddDatasToList(FindObjectsOfType<HelpingHand>());

        AddDatasToList(FindObjectsOfType<Bullet>());
        AddDatasToList(FindObjectsOfType<PlayerBullet>());
        AddDatasToList(FindObjectsOfType<Bullet>());

        var container = new SaveListContainer(){saveDatas = saveDatas};
        container.id = new ObjectId();
        id = container.id;
        database.Save(container);
    }

    private void AddDatasToList(ISaveable[] datas)
    {
        foreach (var data in datas)
        {
            saveDatas.Add(data.GetSaveData());
        }
    }

    public void Load()
    {
        saveDatas = database.Load("5e33059ad7516320501f7671").saveDatas;

        foreach(var data in saveDatas)
        {
            GameObject obj;
            switch (data.type)
            {
                case SaveData.Type.EnemyBullet:
                    obj = Instantiate(EnemyBulletPrefab);
                    break;
                case SaveData.Type.MegaBullet:    
                    obj = Instantiate(MegaBulletPrefab);
                    break;
                case SaveData.Type.PlayerBullet:    
                    obj = Instantiate(PlayerBulletPrefab);
                    break;
                case SaveData.Type.FourArmsEnemy:    
                    obj = Instantiate(FourArmedEnemyPrefab);
                    var fourArmedEnemy = obj.GetComponent<FourArmedEnemy>();
                    fourArmedEnemy.lifes = data.lifes;
                    fourArmedEnemy.targetLocation = data.targetPosition.ToVector2();
                    break;
                case SaveData.Type.ExplodingEnemy:    
                    obj = Instantiate(ExplodingEnemyPrefab);
                    var explodingEnemy = obj.GetComponent<ExplodingEnemy>();
                    explodingEnemy.lifes = data.lifes;
                    explodingEnemy.targetLocation = data.targetPosition.ToVector2();
                    break;
                case SaveData.Type.AimingEnemy:    
                    obj = Instantiate(AimingEnemyPrefab);
                    var aimingEnemy = obj.GetComponent<AimingEnemy>();
                    aimingEnemy.lifes = data.lifes;
                    aimingEnemy.targetLocation = data.targetPosition.ToVector2();
                    break;
                case SaveData.Type.HelpingEnemy:    
                    obj = Instantiate(HelpingHandPrefab);
                    var helpingHand = obj.GetComponent<HelpingHand>();
                    helpingHand.lifes = data.lifes;
                    helpingHand.targetLocation = data.targetPosition.ToVector2();
                    break;
                default:
                    obj = new GameObject();
                    break;
            }

            obj.transform.position = data.position.ToVector2();
            obj.transform.rotation = data.rotation.ToVector4();
            obj.transform.localScale = new Vector3(data.scale, data.scale, data.scale);
        }
    }

*/
}
