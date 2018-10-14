using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class Server : MonoBehaviour {
    public static Server Instance { get; set; }

    private int port;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectClients;

    private TcpListener server;
    private bool isServerStarted;

    public List<Room> listRoom;

    private void Awake()
    {
        Instance = this;
    }

    public bool Init(int port)
    {
        DontDestroyOnLoad(this);
        this.port = port;
        clients = new List<ServerClient>();
        disconnectClients = new List<ServerClient>();

        listRoom = new List<Room>();
        try
        {
            server = new TcpListener(IPAddress.Any ,port);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return false;
    }

    public void StartServer()
    {
        if (server != null)
        {
            server.Start();

            StartListening();

            isServerStarted = true;
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult asyncResult)
    {
        string allUser = "";
        foreach (var sc in clients)
        {
            allUser += sc.clientName + '|';
        }

        TcpListener listener = (TcpListener)asyncResult.AsyncState;

        ServerClient client = new ServerClient(listener.EndAcceptTcpClient(asyncResult), null);

        clients.Add(client);

        StartListening();

        Broadcast(ConstantData.WHO_CONNECTED+'|' + allUser, client);

        Debug.Log("Somebody has connected!");
    }

    private bool IsClientStillConnected(TcpClient tcp)
    {
        try
        {
            if(tcp != null && tcp.Client != null && tcp.Connected)
            {
                if (tcp.Client.Poll(0, SelectMode.SelectRead))
                    return !(tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                return true;
            } 
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
        return false;
    }

    private void Broadcast(string data, List<ServerClient> clients)
    {
        foreach (var sc in clients)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(sc.tcp.GetStream());
                streamWriter.WriteLine(data);
                streamWriter.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    private void Broadcast(string data, ServerClient client)
    {
        List<ServerClient> clients = new List<ServerClient> { client };
        Broadcast(data, clients);
    }

    private void OnInCommingData(ServerClient client, string data)
    {
        Debug.Log(client.clientName + ": "+ data);
        string[] msg = data.Split('|');

        switch (msg[0])
        {
            case ConstantData.WHO_CONNECTED_RESPONSE:
                client.clientName = msg[1];
                int index = clients.Count;
                List<ServerClient> temp = new List<ServerClient>();
                for (int i = 0; i < index - 1; i++)
                {
                    temp.Add(clients[i]);
                }
                Broadcast(ConstantData.ANNOUNCE_WHO_CONNECTED + '|' + client.clientName, temp);
                Broadcast(ConstantData.WELCOME_MESSAGE + "|welcome {0}", clients[index - 1]);
                temp.Clear();
                break;
            case ConstantData.CREATE_ROOM:
                Room newRoom = new Room { roomID = 1 };
                listRoom.Add(newRoom);
                Broadcast(ConstantData.CREATE_ROOM_RESPONSE+ "|success", clients);
                Debug.Log("Create new room success");
                break;
        }
    }

    private void Update()
    {
        if (!isServerStarted) return;

        foreach (var client in clients)
        {
            if (!IsClientStillConnected(client.tcp))
            {
                client.tcp.Close();
                disconnectClients.Add(client);
                Debug.Log(client.clientName + " has disconnected");
                continue;
            }
            else
            {
                NetworkStream networkStream = client.tcp.GetStream();
                if (networkStream.DataAvailable)
                {
                    StreamReader streamReader = new StreamReader(networkStream, true);
                    string data = streamReader.ReadLine();

                    if (!String.IsNullOrEmpty(data))
                    {
                        OnInCommingData(client, data);
                    }
                }
            }
        }

        for (int i = 0; i < disconnectClients.Count; i++)
        {
            clients.Remove(disconnectClients[i]);
            disconnectClients.RemoveAt(i);
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public UdpClient udp;

    public ServerClient(TcpClient tcp, UdpClient udp)
    {
        this.tcp = tcp;
        this.udp = udp;
    }
}
