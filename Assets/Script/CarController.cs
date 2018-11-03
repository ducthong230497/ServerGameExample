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
    private Vector2 previousVelocity;

    private GameObject me;
    private GameObject opponent;

    private bool moveForward;

    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
        client.onUpdateOponentRoation += OnUpdateOpponentRotation;
        client.onUpdatePlayerSpeed += OnUpdateOpponentSpeed;
    }
    // Use this for initialization
    void Start () {
        LogController.Log("string isHost: " + PlayerPrefs.GetString("isHost"));
        bool isHost = Convert.ToBoolean(PlayerPrefs.GetString("isHost"));
        me = client.IsHostRoom ? blueCar : blackCar;
        opponent = client.IsHostRoom ? blackCar : blueCar;
        myRigidbody2D = me.GetComponent<Rigidbody2D>();
        opponentRigidbody2D = opponent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //z += rotateSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, z);
            me.transform.Rotate((Input.GetKey(KeyCode.A) ? Vector3.forward :
                             Input.GetKey(KeyCode.D) ? Vector3.back : Vector3.zero)
                             * rotateSpeed * Time.deltaTime);

            serverObject = new ServerObject();
            serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_ROTATION);
            serverObject.PutInt("roomID", client.LastJoinRoom);
            serverObject.PutFloat("x", me.transform.eulerAngles.x);
            serverObject.PutFloat("y", me.transform.eulerAngles.y);
            serverObject.PutFloat("z", me.transform.eulerAngles.z);
            client.SendData(serverObject);
        }

        if (Input.GetKey(KeyCode.W))
            moveForward = true ;
        else
            moveForward = false;
    }

    private void FixedUpdate()
    {
        myRigidbody2D.velocity = ((moveForward ? me.transform.up : Vector3.zero) * pushForce * Time.deltaTime);
        if (myRigidbody2D.velocity != previousVelocity)
        {
            serverObject = new ServerObject();
            serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_SPEED);
            serverObject.PutInt("roomID", client.LastJoinRoom);
            serverObject.PutFloat("x", myRigidbody2D.velocity.x);
            serverObject.PutFloat("y", myRigidbody2D.velocity.y);
            client.SendData(serverObject);
            previousVelocity = myRigidbody2D.velocity;
        }
        //opponentRigidbody2D.velocity = ((moveForward ? opponent.transform.up : Vector3.zero) * pushForce * Time.deltaTime);
    }

    private void OnUpdateOpponentRotation(float x, float y, float z)
    {
        opponent.transform.rotation = Quaternion.Euler(x, y, z);
    }

    private void OnUpdateOpponentSpeed(float x, float y)
    {
        opponentRigidbody2D.velocity = new Vector2(x, y);
    }
}
