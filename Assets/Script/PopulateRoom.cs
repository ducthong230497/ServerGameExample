using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateRoom : MonoBehaviour {
    [SerializeField] private GameObject Room;

    private GridLayoutGroup gridLayout;
    private Client client;
    private int amount;
    private List<RoomMono> listRoom = new List<RoomMono>();
    private bool isCreateRoom;

	void Start () {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        client.onCreateRoomResponse += OnCreateRoomResponse;
        client.onJoinRoomResponse += OnJoinRoomResponse;
        client.onGetListRoom += OnGetListRoom;

        gridLayout = GetComponent<GridLayoutGroup>();

        float screenSizeIgnorePadding = Screen.width - 2 * gridLayout.padding.left;
        amount = Mathf.FloorToInt(screenSizeIgnorePadding / gridLayout.cellSize.x);
        LogController.Log(amount.ToString(), "amount" );
        float maxUsedSize = gridLayout.cellSize.x * amount;
        float sizeLeftOver = screenSizeIgnorePadding - maxUsedSize;
        float newSpacing = Mathf.FloorToInt(sizeLeftOver / (amount - 1));
        gridLayout.spacing = new Vector2(newSpacing, newSpacing);

        ServerObject so = new ServerObject();
        so.PutString("cmd", ConstantData.GET_LIST_ROOM);
        client.SendData(so);
	}

    private void OnJoinRoomResponse(string isHost, int roomID)
    {
        int host = Convert.ToInt32(isHost);
        client.LastJoinRoom = roomID;
        PlayerPrefs.SetString("clientName", client.ClientName);
        if(host == 1)
        {
            client.IsHostRoom = true;
        }
        else
        {
            client.IsHostRoom = false;
        }

        GameManager.Instance.LoadScene("WaitingRoomScene");
    }

    public void CreateRoom()
    {
        ServerObject so = new ServerObject();
        so.PutString("cmd", ConstantData.CREATE_ROOM);
        client.SendData(so);
        isCreateRoom = true;
    }

    private void OnCreateRoomResponse(string msg, int roomID)
    {
        if (msg.Equals("success"))
        {
            GameObject room = Instantiate(Room, transform);
            room.GetComponent<RoomMono>().room.onRoomClicked = OnRoomClicked;
            room.GetComponent<RoomMono>().room.roomID = roomID;
            listRoom.Add(room.GetComponent<RoomMono>());
            if(isCreateRoom)
            {
                ServerObject so = new ServerObject();
                so.PutString("cmd", ConstantData.JOIN_ROOM);
                so.PutString("clientName", client.ClientName);
                so.PutInt("roomID", roomID);
                client.SendData(so);
                isCreateRoom = false;
            }
        }
        else
            LogController.LogError("Fail to create room");
    }

    private void OnRoomClicked(int roomID)
    {
        ServerObject so = new ServerObject();
        so.PutString("cmd", ConstantData.JOIN_ROOM);
        so.PutString("clientName", client.ClientName);
        so.PutInt("roomID", roomID);
        client.SendData(so);
    }

    private void OnGetListRoom(List<Room> listRoom)
    {
        foreach (var room in listRoom)
        {
            GameObject r = Instantiate(Room, transform);
            r.GetComponent<RoomMono>().room = room;
            r.GetComponent<RoomMono>().room.onRoomClicked = OnRoomClicked;
            //r.GetComponent<RoomMono>().room.roomID = room.roomID;
            //r.GetComponent<RoomMono>().room.client1 = room.client1;
            //r.GetComponent<RoomMono>().room.client2 = room.client2;
            this.listRoom.Add(r.GetComponent<RoomMono>());
        }
    }
}
