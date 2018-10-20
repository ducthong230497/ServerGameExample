using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingRoomController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI TMPro_client1;
    [SerializeField] private TextMeshProUGUI TMPro_client2;

    private Client client;
    private bool isHost;
    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
    }

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("isHost"))
        {
            isHost = Convert.ToBoolean(PlayerPrefs.GetString("isHost"));
            if(isHost)
                TMPro_client1.text = PlayerPrefs.GetString("clientName");
            else
            {
                TMPro_client2.text = PlayerPrefs.GetString("clientName");
                client.SendData(ConstantData.GET_ROOM_INFO + "|" + client.LastJoinRoom);
            }
        }

        
	}

    private void GetRoomInfo()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
