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
    private List<Room> listRoom;

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
            Instantiate(Room, transform);
        }
	}

    private void OnJoinRoomResponse(string isHost)
    {
        int host = Convert.ToInt32(isHost);
        StartCoroutine(MoveToWaitingRoom());
        if(host == 1)
        {

        }
    }

    public void CreateRoom()
    {
        client.SendData(ConstantData.CREATE_ROOM);
    }

    private void OnCreateRoomResponse(string msg, int roomID)
    {
        if (msg.Equals("success"))
        {
            Instantiate(Room, transform);
            client.SendData(ConstantData.JOIN_ROOM + "|"+client.ClientName+"|"+roomID);
        }
        else
            Debug.Log("Fail to create room");
    }

    private IEnumerator MoveToWaitingRoom()
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.LoadScene("WaitingRoomScene");
    }
}
