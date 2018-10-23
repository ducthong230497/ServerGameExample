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

        gridLayout = GetComponent<GridLayoutGroup>();

        float screenSizeIgnorePadding = Screen.width - 2 * gridLayout.padding.left;
        amount = Mathf.FloorToInt(screenSizeIgnorePadding / gridLayout.cellSize.x);
        Debug.Log("amount: " + amount);
        float maxUsedSize = gridLayout.cellSize.x * amount;
        float sizeLeftOver = screenSizeIgnorePadding - maxUsedSize;
        float newSpacing = Mathf.FloorToInt(sizeLeftOver / (amount - 1));
        gridLayout.spacing = new Vector2(newSpacing, newSpacing);

        //client.SendData(ConstantData.GET_LIST_ROOM);
        foreach (var roomMono in Server.Instance.listRoom)
        {
            GameObject r = Instantiate(Room, transform);
            r.GetComponent<RoomMono>().room.onRoomClicked = OnRoomClicked;
            r.GetComponent<RoomMono>().room.roomID = roomMono.room.roomID;
            listRoom.Add(r.GetComponent<RoomMono>());
        }
	}

    private void OnJoinRoomResponse(string isHost, int roomID)
    {
        int host = Convert.ToInt32(isHost);
        client.LastJoinRoom = roomID;
        PlayerPrefs.SetString("clientName", client.ClientName);
        if(host == 1)
        {
            PlayerPrefs.SetString("isHost", "true");
        }
        else
        {
            PlayerPrefs.SetString("isHost", "false");
        }
        StartCoroutine(MoveToWaitingRoom());
    }

    public void CreateRoom()
    {
        client.SendData(ConstantData.CREATE_ROOM);
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
                client.SendData(ConstantData.JOIN_ROOM + "|" + client.ClientName + "|" + roomID);
                isCreateRoom = false;
            }
        }
        else
            Debug.Log("Fail to create room");
    }

    private IEnumerator MoveToWaitingRoom()
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.LoadScene("WaitingRoomScene");
    }

    private void OnRoomClicked(int roomID)
    {
        client.SendData(ConstantData.JOIN_ROOM + "|" + client.ClientName + "|" + roomID);
    }
}
