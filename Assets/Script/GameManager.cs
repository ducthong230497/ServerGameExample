using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; set; }
    // Use this for initialization
    [SerializeField] private int port = 8080;
    public GameObject serverPref;
    public GameObject clientPref;
    [Space(5)]
    public TextMeshProUGUI interactText;
    public Text name;

    internal Server server;
    
    private string hostAddress = "127.0.0.1";
    
    void Start () {
        Instance = this;
        DontDestroyOnLoad(this);
        //InitServer();
	}
	
    private void InitServer()
    {
        try
        {
            server = Instantiate(serverPref).GetComponent<Server>();
            if (!server.Init(port))
            {
                LogController.LogError("Can not initialize server");
                return;
            }
            server.StartServer();
            interactText.SetText("Listening");
        }
        catch (SocketException e)
        {
            LogController.LogError(e.Message);
        }
    }

    public void ConnectButton()
    {

        try
        {
            Client client = Instantiate(clientPref).GetComponent<Client>();
            client.ClientName = string.IsNullOrEmpty(name.text) ? "client" : name.text;
            client.ConnectToServer(hostAddress, port);
            interactText.SetText("Connecting");

        }
        catch (Exception e)
        {
            LogController.LogError(e.Message);
        }
    }

    public void HostButton()
    {
        try
        {
            Server server = Instantiate(serverPref).GetComponent<Server>();
            if (!server.Init(port))
            {
                LogController.LogError("Can not initialize server");
                return;
            }
            server.StartServer();
            interactText.SetText("Listening");
            //Client client = Instantiate(clientPref).GetComponent<Client>();
            //client.ClientName = string.IsNullOrEmpty(name.text) ? "client" : name.text;
            //client.ConnectToServer(hostAddress, port);
        }
        catch (Exception e)
        {
            LogController.LogError(e.Message);
        }

        try
        {
            Room r = new Room { roomID = 1997 };

            MemoryStream memoryStream = new MemoryStream();

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, r);

            byte[] buffer = memoryStream.ToArray();

            byte[] clientData = new byte[256];

            BinaryFormatter formattor = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(buffer);
            Room objFileInfo = (Room)formattor.Deserialize(ms);

            int a = 2;
        }
        catch (Exception e)
        {
            LogController.LogError(e.Message);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
