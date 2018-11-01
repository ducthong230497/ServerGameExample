using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    [SerializeField] GameObject blackCar;
    [SerializeField] GameObject blueCar;
    [SerializeField] private int pushForce = 100;
    [SerializeField] private int rotateSpeed = 50;

    private Client client;
    private ServerObject serverObject;

    private Rigidbody2D myRigidbody2D;
    private Rigidbody2D opponentRigidbody2D;

    private GameObject me;
    private GameObject opponent;

    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        client.onUpdateOponentRoation += OnUpdateOpponentRotation;
    }
    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("isHost"))
        {
            bool isHost = Convert.ToBoolean(PlayerPrefs.GetString("isHost"));
            me = isHost ? blueCar : blackCar;
            opponent = isHost ? blackCar : blueCar;
            myRigidbody2D = me.GetComponent<Rigidbody2D>();
            opponentRigidbody2D = opponent.GetComponent<Rigidbody2D>();
        }
    }

    float z;
    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            //z += rotateSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, z);
            me.transform.Rotate((Input.GetKey(KeyCode.LeftArrow) ? Vector3.forward :
                             Input.GetKey(KeyCode.RightArrow) ? Vector3.back : Vector3.zero)
                             * rotateSpeed * Time.deltaTime);
            serverObject = new ServerObject();
            serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_ROTATION);
            serverObject.PutInt("roomID", client.LastJoinRoom);
            serverObject.PutFloat("x", me.transform.eulerAngles.x);
            serverObject.PutFloat("y", me.transform.eulerAngles.y);
            serverObject.PutFloat("z", me.transform.eulerAngles.z);
            client.SendData(serverObject);
        }
        //else if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    //z -= rotateSpeed * Time.deltaTime;
        //    //transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, z);
        //    transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
        //    serverObject = new ServerObject();
        //    serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_ROTATION);
        //    serverObject.PutInt("roomID", client.LastJoinRoom);
        //    serverObject.PutFloat("x", transform.eulerAngles.x);
        //    serverObject.PutFloat("y", transform.eulerAngles.y);
        //    serverObject.PutFloat("z", transform.eulerAngles.z);
        //    client.SendData(serverObject);
        //}
    }

    private void FixedUpdate()
    {
        myRigidbody2D.velocity = (me.transform.up * pushForce * Time.deltaTime);
        opponentRigidbody2D.velocity = (opponent.transform.up * pushForce * Time.deltaTime);
    }

    private void OnUpdateOpponentRotation(float x, float y, float z)
    {
        opponent.transform.rotation = Quaternion.Euler(x, y, z);
        //if (PlayerPrefs.HasKey("isHost"))
        //{
        //    bool isHost = Convert.ToBoolean(PlayerPrefs.GetString("isHost"));
        //    if(isHost)
        //    {
        //        opponentCar.transform.rotation = Quaternion.Euler(x, y, z);
        //    }
        //    else
        //    {
        //        transform.rotation = Quaternion.Euler(x, y, z);
        //    }
        //}
    }
}
