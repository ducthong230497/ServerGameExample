using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class Server : MonoBehaviour {
    [SerializeField] private int port = 8080;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectClients;

    private TcpListener server;
    private bool isServerStarted;

    public void Init()
    {
        DontDestroyOnLoad(this);
        clients = new List<ServerClient>();
        disconnectClients = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any ,port);
            server.Start();

            StartListening();
            isServerStarted = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult asyncResult)
    {
        TcpListener listener = (TcpListener)asyncResult.AsyncState;

        ServerClient client = new ServerClient(listener.EndAcceptTcpClient(asyncResult), null);

        clients.Add(client);

        Debug.Log("Somebody has connected!");
    }

    private bool IsClientStillConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
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

    private void OnInCommingData(ServerClient client, string data)
    {
        Debug.Log(client.clientName + ": "+ data);
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
