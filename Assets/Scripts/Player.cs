using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this automatically adds the Controller2D-script component to the player
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    // adding gravity float to the player object
    public float gravity = -20;
    public float moveSpeed = 6;
    // reference to the player's velocity
    Vector3 velocity;

    // reference to the controller2d script
    Controller2D controller;

    void Start () {
        // gets the 2d controller component and stores it in the controller reference
        controller = GetComponent<Controller2D>();
	}
	
	void Update () {

        // stops the player from accumulating gravity if not moving on the Y-axis
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        // gets the raw input of the virtual axes, no smoothing filter
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //
        velocity.x = input.x * moveSpeed;
        // applying the gravity to the y-velocity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}
}
