using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    [SerializeField] GameObject blackCar;
    [SerializeField] GameObject blueCar;
    [SerializeField] private int pushForce = 10;
    [SerializeField] private int rotateSpeed = 10;
    public bool canControll;

    private Client client;
    private ServerObject serverObject;

    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
    }
    // Use this for initialization
    void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}

    float z;
    // Update is called once per frame
    void Update () {
        //Quaternion quaternion = transform.rotation;
        Debug.Log(transform.eulerAngles);
        if (canControll)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //z += rotateSpeed * Time.deltaTime;
                //transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, z);
                transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
                serverObject = new ServerObject();
                serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_ROTATION);
                serverObject.PutInt("roomID", client.LastJoinRoom);
                serverObject.PutFloat("x", transform.eulerAngles.x);
                serverObject.PutFloat("y", transform.eulerAngles.y);
                serverObject.PutFloat("z", transform.eulerAngles.z);
                client.SendData(serverObject);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                //z -= rotateSpeed * Time.deltaTime;
                //transform.rotation = Quaternion.Euler(quaternion.x, quaternion.y, z);
                transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
                serverObject = new ServerObject();
                serverObject.PutString("cmd", ConstantData.UPDATE_PLAYER_ROTATION);
                serverObject.PutInt("roomID", client.LastJoinRoom);
                serverObject.PutFloat("x", transform.eulerAngles.x);
                serverObject.PutFloat("y", transform.eulerAngles.y);
                serverObject.PutFloat("z", transform.eulerAngles.z);
                client.SendData(serverObject);
            }
        }
	}

    private void FixedUpdate()
    {
        rigidbody2D.velocity = (transform.up * pushForce * Time.deltaTime);
    }

    private void OnUpdateOpponentRotation(float x, float y, float z)
    {
        if (PlayerPrefs.HasKey("isHost"))
        {
            bool isHost = Convert.ToBoolean(PlayerPrefs.GetString("isHost"));
            if(isHost)
            {
                blackCar.transform.rotation = Quaternion.Euler(x, y, z);
            }
            else
            {
                blueCar.transform.rotation = Quaternion.Euler(x, y, z);
            }
        }
    }
}
