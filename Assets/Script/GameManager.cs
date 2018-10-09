using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    
    private string hostAddress = "127.0.0.1";

    static int i = 0;
    void Start () {
        Instance = this;
        DontDestroyOnLoad(this);
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
            Debug.LogError(e.Message);
        }
    }

    public void HostButton()
    {
        try
        {
            Server server = Instantiate(serverPref).GetComponent<Server>();
            if (!server.Init(port))
            {
                Debug.LogError("Can not initialize server");
                return;
            }
            server.StartServer();
            interactText.SetText("Listening");
            Client client = Instantiate(clientPref).GetComponent<Client>();
            client.ClientName = string.IsNullOrEmpty(name.text) ? "client" : name.text;
            client.ConnectToServer(hostAddress, port);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
