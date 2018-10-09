using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour {

    private bool isSocketReady;
    private TcpClient socket;
    private NetworkStream networkStream;
    private StreamWriter streamWriter;
    private StreamReader streamReader;
    public string ClientName { get; set; }

    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(this);
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

    private void SendData(string data)
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