using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTest : MonoBehaviour {
    
    public Transform target;
    public float hoverAcc = 5;
    public float hoverDecel = -2;
    public float terminalVel = 10f;

    public float startingVelocity = 0;
    public float baseAcceleration = -1;
    

    private float velocity = 0;
    private float decelDistance, distance;

    private void Start() {
        velocity = startingVelocity;
    }

    private void Update() {
        //Apply gravity:
        velocity = velocity + baseAcceleration * Time.deltaTime;

        // Move the object, given its velocity.
        transform.Translate(Vector3.up * velocity * Time.deltaTime);

        // PRETEND PLAYERMOVEMENT IS ABOVE ^, HOVER IS BELOW \/

        // Only do hover if we are pressing space.
        if (!Input.GetKey(KeyCode.Space)) {
            return;
        }

        // Distance to target height.
        distance = GetDistance(transform.position.y, target.position.y);

        // If we can cover that distance this frame with our current velocity, just get there already.
        if (distance <= velocity * Time.deltaTime) {
            velocity = 0;
            transform.position = target.position;
            return;
        }

        // Undo gravity?
        // velocity = velocity - baseAcceleration * Time.deltaTime;

        // The minimum distance needed to smoothly stop at a point, given our current velocity and the constant deceleration.
        decelDistance = CalculatePositionDelta(0, velocity, hoverDecel);

        // If we are outside the deceleration zone, accelerate, otherwise... well, decelerate.        
        if (distance > decelDistance) {
            velocity = velocity + (hoverAcc) * Time.deltaTime; 
        } else {
            Debug.Log("Decel");
            velocity = velocity + (hoverDecel) * Time.deltaTime;
        }
    }

    // Standard kinematic equation
    private float CalculatePositionDelta(float finalVelocity, float initialVelocity, float acceleration) {
        return (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * acceleration);
    }

    private float GetDistance(float currentPosition, float targetPosition) {
        return targetPosition - currentPosition;
    }



}
