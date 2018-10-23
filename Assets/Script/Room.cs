using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Room : ISerializable
{
    public int roomID { get; set; }
    public int numberPlayer;
    public ServerClient client1;
    public ServerClient client2;
    public Action<int> onRoomClicked;

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("roomID", roomID, typeof(int));
    }

    public Room(SerializationInfo info, StreamingContext context)
    {
        roomID = info.GetInt32("roomID");
    }

    public Room()
    {

    }
}
