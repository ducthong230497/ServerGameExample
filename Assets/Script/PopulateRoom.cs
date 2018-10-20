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
    private List<Room> listRoom = new List<Room>();
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

        foreach (var room in Server.Instance.listRoom)
        {
            GameObject r = Instantiate(Room, transform);
            r.GetComponent<Room>().onRoomClicked = OnRoomClicked;
            r.GetComponent<Room>().roomID = room.roomID;
            listRoom.Add(r.GetComponent<Room>());
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
            room.GetComponent<Room>().onRoomClicked = OnRoomClicked;
            room.GetComponent<Room>().roomID = roomID;
            listRoom.Add(room.GetComponent<Room>());
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
