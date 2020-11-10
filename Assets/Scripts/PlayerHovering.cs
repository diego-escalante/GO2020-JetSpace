using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHovering : MonoBehaviour {

    //TODO: This is a good start, but I want a longer burn time and higher acceleration WITHOUT worrying about
    // the player gaining more than 1 block height. In other words, after a certain height gain, taper off
    // smoothly to a balanced hover. This will take a lot of tweaks to get right.
    // This is emerging as the key mechanic, and it needs to feel good.

    public float maxHoverHeightGain = 3f;
    public float hoverAcc = 35f;
    public float burnTime = 2f;
    private float burnTimeLeft;

    private PlayerMovement playerMovement;
    private bool hoverEnabled = false;
    private float hoverDecel;
    private float targetHeight;

    private void Start() {
        playerMovement = GetComponent<PlayerMovement>();
        hoverDecel = playerMovement.GetGravity();
    }

    private void Update() {
        // Temporary: If we are grounded, refill burn time.
        if (playerMovement.IsGrounded()) {
            burnTimeLeft = burnTime;
            hoverEnabled = false;
        } 
        
        if (playerMovement.JumpsLeft() == 0 && !playerMovement.IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
            hoverEnabled = true;
            targetHeight = playerMovement.GetLastGroundedPosition().y + playerMovement.GetJumpHeight() + maxHoverHeightGain;
        } else if (hoverEnabled && Input.GetKeyUp(KeyCode.Space)) {
            hoverEnabled = false;
        }

        if (hoverEnabled && burnTimeLeft > 0) {
            burnTimeLeft -= Time.deltaTime;

            // Distance to target height.
            float distance = GetDistance(transform.position.y, targetHeight);
            Vector3 velocity = playerMovement.GetVelocity();

            // If we are at the target height (or above it) or if we can cover the distance this frame with the current
            // velocity to reach the target, just get to the target already and stop y-velocity.
            if (transform.position.y > targetHeight || distance <= velocity.y * Time.deltaTime) {
                // Stop y-velocity. Just cancel it with its negative.
                playerMovement.AddVelocity(new Vector3(0, -velocity.y, 0));
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
                return;
            }

            // The minimum distance needed to smoothly stop at a point, given our current velocity and the constant deceleration.
            float decelDistance = CalculatePositionDelta(0, velocity.y, hoverDecel);

            // If we are outside the deceleration zone, accelerate, otherwise... well, decelerate.
            playerMovement.AddVelocity(Vector3.up * Time.deltaTime * (distance > decelDistance ? hoverAcc : hoverDecel));        

        }    
    }
    
    private float GetDistance(float currentPosition, float targetPosition) {
        return targetPosition - currentPosition;
    }

    // Standard kinematic equation
    private float CalculatePositionDelta(float finalVelocity, float initialVelocity, float acceleration) {
        return (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * acceleration);
    }
}
