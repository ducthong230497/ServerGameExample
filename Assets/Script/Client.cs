using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

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
    public Action<string, int> onJoinRoomResponse;
    public Action<string> onPlayerJoinRoomResponse;
    public Action<string, string> onGetRoomInfoResponse;
    public Action<List<Room>> onGetListRoom;
    public Action onGuestReady;
    public Action onStartGame;
    public Action<float, float, float> onUpdateOponentRoation;

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

    public void SendData(ServerObject so)
    {
        if (!isSocketReady) return;

        MemoryStream ms = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(ms, so);

        byte[] buffer = ms.ToArray();

        networkStream.Write(buffer, 0, buffer.Length);
    }

    private void OnInComingData(string data)
    {
        Debug.Log(data);
        string[] msg = data.Split('|');

        switch (msg[0])
        {
            case ConstantData.WHO_CONNECTED:
                for (int i = 1; i < msg.Length - 1; i++)
                {
                    UserConnected(msg[i], false);
                }
                SendData(ConstantData.WHO_CONNECTED_RESPONSE + '|' + ClientName);
                break;
            case ConstantData.ANNOUNCE_WHO_CONNECTED:
                UserConnected(msg[1], false);
                //announceText.text = data;
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
            case ConstantData.GET_ROOM_INFO_RESPONSE:
                onGetRoomInfoResponse(msg[1], msg[2]);
                break;
            case ConstantData.GUEST_READY:
            case ConstantData.GUEST_CANCLE_READY:
                onGuestReady();
                break;
            case ConstantData.START_GAME:
                onStartGame();
                break;
        }
    }

    private void OnInComingData(ServerObject so)
    {
        ServerObject serverObject = new ServerObject();

        Debug.Log(so);
        string cmd = so.GetString("cmd");
        string[] msg = null;

        switch (cmd)
        {
            case ConstantData.WHO_CONNECTED:
                List<string> userName = so.GetList<string>("usersName");
                for (int i = 1; i < userName.Count - 1; i++)
                {
                    UserConnected(userName[i], false);
                }
                serverObject.PutString("cmd", ConstantData.WHO_CONNECTED);
                serverObject.PutString("clientName", ClientName);
                SendData(serverObject);
                break;
            case ConstantData.ANNOUNCE_WHO_CONNECTED:
                UserConnected(so.GetString("clientName"), false);
                //announceText.text = data;
                break;
            case ConstantData.WELCOME_MESSAGE:
                announceText.text = string.Format(so.GetString("welcomemsg"), ClientName);
                StartCoroutine(MoveToLobby());
                break;
            case ConstantData.CREATE_ROOM:
                onCreateRoomResponse(so.GetString("msg"), so.GetInt("roomID"));
                break;
            case ConstantData.JOIN_ROOM:
                onJoinRoomResponse(so.GetString("isHost"), so.GetInt("roomID"));
                break;
            case ConstantData.GET_LIST_ROOM:
                onGetListRoom(so.GetList<Room>("listRoom"));
                break;
            case ConstantData.GET_ROOM_INFO:
                onGetRoomInfoResponse(so.GetString("client1"), so.GetString("client2"));
                break;
            case ConstantData.GUEST_READY:
            case ConstantData.GUEST_CANCLE_READY:
                onGuestReady();
                break;
            case ConstantData.START_GAME:
                onStartGame();
                break;
            case ConstantData.UPDATE_PLAYER_ROTATION:
                onUpdateOponentRoation(so.GetFloat("x"), so.GetFloat("y"), so.GetFloat("z"));
                break;
        }
    }

    private void Update()
    {
        if (isSocketReady)
        {
            if (networkStream.DataAvailable)
            {
                //string data = streamReader.ReadLine();

                byte[] readBuffer = new byte[socket.ReceiveBufferSize];
                byte[] temp = null;

                int numberOfBytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
                if (numberOfBytesRead <= 0)
                {
                    return;
                }
                temp = new byte[numberOfBytesRead];

                Array.Copy(readBuffer, 0, temp, 0, numberOfBytesRead);

                using (MemoryStream ms = new MemoryStream(temp))
                {
                    //fullServerReply = Encoding.UTF8.GetString(writer.ToArray());
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    ServerObject so = (ServerObject)binaryFormatter.Deserialize(ms);
                    int a = 2;
                    OnInComingData(so);
                }

                //if (!string.IsNullOrEmpty(data))
                //{
                //    OnInComingData(data);
                //}
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