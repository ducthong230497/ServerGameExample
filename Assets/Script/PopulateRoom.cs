using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateRoom : MonoBehaviour {
    [SerializeField] private GameObject Room;
    private GridLayoutGroup gridLayout;
    private Client client;
    private int amount;
	void Start () {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        client.onCreateRoomResponse += OnCreateRoomResponse;
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
	
	public void CreateRoom()
    {
        client.SendData(ConstantData.CREATE_ROOM);
    }

    private void OnCreateRoomResponse(string msg)
    {
        if (msg.Equals("success"))
        {
            Instantiate(Room, transform);
            StartCoroutine(MoveToWaitingRoom());
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
