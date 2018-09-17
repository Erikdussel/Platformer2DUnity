using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this automatically adds the BoxCollider2D component to objects using the Controller2D script
[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask collisionMask;
    // adding a little inset for the boxcollider to make sure the raycast can still fire when the object is flat on the ground/platform
    const float boxInset = .015f;

    // defining the amount of rays fired horizontaly and vertically
    public int rayCountHorizontal = 4;
    public int rayCountVertical = 4;
    // space between the rays
    float horizontalRaySpacing;
    float verticalRaySpacing;

    // references
    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    public CollisionData collisions;

    void Start ()
    {
        // gets the BoxCollider2D component and stores it in the collider reference
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }
    // Method for Movement
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        // handling the collision modifies velocity to not go through objects;
        if (velocity.x != 0)
        {
            HorizontalCollision(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollision(ref velocity);
        }

        // finally transform the object
        transform.Translate(velocity);
    }

    // takes in a reference instead of a copy of the variable - any change inside the VerticalCollision will change the velocity passed inside the Move() method
    void VerticalCollision(ref Vector3 velocity)
    {
        // get the direction of the y velocity
        // Mathf.Sign(n) returns 1 if 'n' is a positive value and returns -1 if 'n' is a negative value
        float directionY = Mathf.Sign(velocity.y);
        // get the length of the ray including the inset of the boxcollider;
        // Mathf.Abs(n) returns the absolute value of 'n', so it always returns a positive value
        float rayLength = Mathf.Abs(velocity.y) + boxInset;

        // creating the vertical raycasts
        for (int i = 0; i < rayCountVertical; i++)
        {
            // if the object is moving down set the raycast origin to the bottom left corner, else set the raycast origin to topleft;
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            // add the velocity on the x-axis to cast the vertical rays to the position the object will be
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            // RaycastHit2D to detect objects along the path of the ray;
            // Physics2D.Raycast(Vector3 origin, Vector2 direction, distance, layermask);
            // the collisionMask in this case is a 'platform layer' added in Unity
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            // DrawRay(Vector3 start, Vector3 dir, color of the rays)
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - boxInset) * directionY;
                // set the rayLength to the distance of the closest hit so that it wont set the rayLength to a ray hit with a larger distance when the loop iterates further.
                // e.g. when it is over multiple platforms with different heights, it will collide with the highest platform and not with the lower platform that the raycast hits.
                rayLength = hit.distance;
                // if the object hit something and moves down, collisions.below is true;
                collisions.below = directionY == -1;
                // if the object hit something and moves up, collisions.above is true;
                collisions.above = directionY == 1;
            }
        }
    }
    // does the same as VerticalCollision, but then horizontally
    void HorizontalCollision(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + boxInset;

        for (int i = 0; i < rayCountHorizontal; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);

            if (hit)
            {
                velocity.x = (hit.distance - boxInset) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        // gets the bounds of the Boxcollider2D
        Bounds bounds = collider.bounds;
        // Expands the bounds with a negative value to shrink it slightly;
        bounds.Expand(boxInset * -2);

        // setting up the corner position;
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(boxInset * -2);

        // there need to be at least one raycast in each corner, so at least 2 for the vertical and horizontal direction
        rayCountHorizontal = Mathf.Clamp(rayCountHorizontal, 2, int.MaxValue);
        rayCountVertical = Mathf.Clamp(rayCountVertical, 2, int.MaxValue);

        // calculating the space between the raycasts on the bounds the BoxCollider2D
        horizontalRaySpacing = bounds.size.x / (rayCountHorizontal - 1);
        verticalRaySpacing = bounds.size.y / (rayCountVertical - 1);
    }

    // struct to store the positions of the corners of the Boxcollider2D
	struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionData
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
