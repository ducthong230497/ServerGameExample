using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

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
        //string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        //Debug.Log(str);
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "test");
        //Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));

        // Load the Scriptable Singleton from disk. (it should only have one entry but YAML supports more then one)
        Object[] loadedObjects = InternalEditorUtility.LoadSerializedFileAndForget("ProjectSettings/ProjectSettings.asset");
        loadedObjects = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
        // Create a serialized object to let use change it's settings. 
        SerializedObject projectSettings = new SerializedObject(loadedObjects[0]);
        // Find the property we need to modify
        SerializedProperty symbols = projectSettings.FindProperty("scriptingDefineSymbols");
        // Add a new element to the end
        //symbols.InsertArrayElementAtIndex(symbols.arraySize);
        // Set the new element. 
        //symbols.GetArrayElementAtIndex(symbols.arraySize - 1) = "RR_DEV";
        string value = symbols.stringValue;
        symbols.stringValue = "test";
        // Apply changes. 
        projectSettings.ApplyModifiedProperties();
        // Save it back to disk.
        //InternalEditorUtility.SaveToSerializedFileAndForget(projectSettings.targetObjects, "ProjectSettings.asset", true);

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
