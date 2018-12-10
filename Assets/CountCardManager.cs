using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCardManager : MonoBehaviour {

    private Client client;
    private ServerObject serverObject;

    private void Awake()
    {
        client = GameObject.Find("Client(Clone)").GetComponent<Client>();
    }

    private void InitCard()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
