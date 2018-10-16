using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public int roomID { get; set; }
    public int numberPlayer;
    public ServerClient client1;
    public ServerClient client2;
    public Action<int> onRoomClicked;
    
    public void OnRoomClicked()
    {
        onRoomClicked.Invoke(roomID);
    }
}
