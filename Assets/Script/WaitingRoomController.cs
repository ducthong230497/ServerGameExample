using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI TMPro_client1;
    [SerializeField] private TextMeshProUGUI TMPro_client2;
    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private Button btnReadyStart;
    private Text startReadyText;

    private Client client;
    private bool isReady;
    private string start = "Start";
    private string ready = "Ready";
    private string cancle = "Cancle";

    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        client.onGetRoomInfoResponse += GetRoomInfo;
        client.onGuestReady += GuestReady;
        client.onStartGame += StartGame;

        startReadyText = btnReadyStart.transform.GetChild(0).GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
        if (client.IsHostRoom)
        {
            TMPro_client1.text = PlayerPrefs.GetString("clientName");
            startReadyText.text = start;
            btnReadyStart.interactable = false;
            btnReadyStart.onClick.AddListener(OnStartClicked);
        }
        else
        {
            TMPro_client2.text = PlayerPrefs.GetString("clientName");
            ServerObject so = new ServerObject();
            so.PutString("cmd", ConstantData.GET_ROOM_INFO);
            so.PutInt("roomID", client.LastJoinRoom);
            client.SendData(so);
            startReadyText.text = ready;
            btnReadyStart.interactable = true;
            btnReadyStart.onClick.AddListener(OnReadyClicked);
        }
    }

    private void GetRoomInfo(string client1, string client2)
    {
        if(client.IsHostRoom)
        {
            TMPro_client2.text = client2;
        }
        else
        {
            TMPro_client1.text = client1;
        }
        
    }

    private void GuestReady()
    {
        btnReadyStart.interactable = !btnReadyStart.interactable;
    }

    private void StartGame()
    {
        GameManager.Instance.LoadScene("CountCardScene");
    }

    public void OnStartClicked()
    {
        ServerObject so = new ServerObject();
        so.PutString("cmd", ConstantData.START_GAME);
        so.PutInt("roomID", client.LastJoinRoom);
        client.SendData(so);
    }
	
	public void OnReadyClicked()
    {
        if (!isReady)
        {
            ServerObject so = new ServerObject();
            so.PutString("cmd", ConstantData.GUEST_READY);
            so.PutInt("roomID", client.LastJoinRoom);
            client.SendData(so);
            startReadyText.text = cancle;
            isReady = true;
            ReadyText.text = ready;
        }
        else
        {
            ServerObject so = new ServerObject();
            so.PutString("cmd", ConstantData.GUEST_CANCLE_READY);
            so.PutInt("roomID", client.LastJoinRoom);
            client.SendData(so);
            startReadyText.text = ready;
            isReady = false;
            ReadyText.text = string.Empty;
        }
    }
}
