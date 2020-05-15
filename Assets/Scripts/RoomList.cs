using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Animations;

public class RoomList : MonoBehaviour
{
    public GameObject listEntryPrefab;
    
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetRooms(List<RoomInfo> roomList)
    {
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var room in roomList)
        {
            var instance = Instantiate(listEntryPrefab);
            instance.transform.parent = transform;
            instance.GetComponent<RoomListEntry>().DisplayRoomInfo(room);
        }
    }
}
