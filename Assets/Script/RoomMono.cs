    using UnityEngine;

public class RoomMono : MonoBehaviour {
    public Room room;
    
    public void OnRoomClicked()
    {
        room.onRoomClicked.Invoke(room.roomID);
    }
}
