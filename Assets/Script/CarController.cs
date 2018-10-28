using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    [SerializeField] private int pushForce = 10;

    private Rigidbody2D rigidbody2D;
	// Use this for initialization
	void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody2D.rotation++;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody2D.rotation--;
        }
	}

    private void FixedUpdate()
    {
        rigidbody2D.AddForce(Vector3.up * pushForce * Time.deltaTime);
    }
}
