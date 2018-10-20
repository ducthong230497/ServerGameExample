using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour {

    private bool isSocketReady;
    private TcpClient socket;
    private NetworkStream networkStream;
    private StreamWriter streamWriter;
    private StreamReader streamReader;
    public string ClientName { get; set; }
    public int LastJoinRoom { get; set; }

    private List<GameClient> players = new List<GameClient>();
    private GameManager gameManager;

    public Action<string, int> onCreateRoomResponse;
    public Action<string, int>      onJoinRoomResponse;
    public Action<string>      onPlayerJoinRoomResponse;

    private Text announceText;
    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        announceText = GameObject.Find("AnounceText").GetComponent<Text>();
    }

    public bool ConnectToServer(string host, int port)
    {
        if (isSocketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            networkStream = socket.GetStream();
            streamWriter = new StreamWriter(networkStream);
            streamReader = new StreamReader(networkStream);

            isSocketReady = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return isSocketReady;
    }

    public void SendData(string data)
    {
        if (!isSocketReady) return;

        streamWriter.WriteLine(data);
        streamWriter.Flush();
    }

    private void OnInComingData(string data)
    {
        Debug.Log(data);
        string[] msg = data.Split('|');

        switch(msg[0])
        {
            case ConstantData.WHO_CONNECTED:
                for (int i = 1; i < msg.Length - 1; i++)
                {
                    UserConnected(msg[i], false);
                }
                SendData(ConstantData.WHO_CONNECTED_RESPONSE+'|'+ClientName);
                break;
            case ConstantData.ANNOUNCE_WHO_CONNECTED:
                UserConnected(msg[1], false);
                announceText.text = data;
                break;
            case ConstantData.WELCOME_MESSAGE:
                announceText.text = string.Format(msg[1], ClientName);
                StartCoroutine(MoveToLobby());
                break;
            case ConstantData.CREATE_ROOM_RESPONSE:
                onCreateRoomResponse(msg[1], Convert.ToInt32(msg[2]));
                break;
            case ConstantData.JOIN_ROOM_RESPONSE:
                onJoinRoomResponse(msg[1], Convert.ToInt32(msg[2]));
                break;
        }
    }

    private void Update()
    {
        if (isSocketReady)
        {
            if (networkStream.DataAvailable)
            {
                string data = streamReader.ReadLine();

                if(!string.IsNullOrEmpty(data))
                {
                    OnInComingData(data);
                }
            }
        }
    }

    private IEnumerator MoveToLobby()
    {
        yield return new WaitForSeconds(2);
        gameManager.LoadScene("LobbyScene");
    }

    private void UserConnected(string name, bool host)
    {
        GameClient player = new GameClient();
        player.name = name;
        player.isHost = host;
        players.Add(player);

        
    }

    private void CloseSocket()
    {
        if (!isSocketReady)
            return;

        streamWriter.Close();
        streamReader.Close();
        socket.Close();

        isSocketReady = false;
    }

    private void OnDisable()
    {
        CloseSocket();
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}