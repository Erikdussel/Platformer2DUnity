using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this automatically adds the Controller2D-script component to the player
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    // adding gravity float to the player object
    float gravity = -20;
    // reference to the player's velocity
    Vector3 velocity;

    // reference to the controller2d script
    Controller2D controller;

    void Start () {
        // gets the 2d controller component and stores it in the controller reference
        controller = GetComponent<Controller2D>();
	}
	
	void Update () {
        // applying the gravity to the velocity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}
}
