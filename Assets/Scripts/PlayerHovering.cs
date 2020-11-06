using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHovering : MonoBehaviour {

    //TODO: This is a good start, but I want a longer burn time and higher acceleration WITHOUT worrying about
    // the player gaining more than 1 block height. In other words, after a certain height gain, taper off
    // smoothly to a balanced hover. This will take a lot of tweaks to get right.
    // This is emerging as the key mechanic, and it needs to feel good.

    public float burnTime = 0.5f;
    private float burnTimeLeft;

    public Vector3 hoverAcceleration = Vector3.up * 35f; 
    private PlayerMovement playerMovement;
    private bool hoverEnabled = false;

    private void Start() {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() {
        // Temporary: If we are grounded, refill burn time.
        if (playerMovement.IsGrounded()) {
            burnTimeLeft = burnTime;
        } 
        
        if (playerMovement.JumpsLeft() == 0 && !playerMovement.IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
            hoverEnabled = true;
        } else if (hoverEnabled && Input.GetKeyUp(KeyCode.Space)) {
            hoverEnabled = false;
        }

        if (hoverEnabled && burnTimeLeft > 0) {
            burnTimeLeft -= Time.deltaTime;
            playerMovement.AddVelocity(hoverAcceleration * Time.deltaTime);
        }
    }
}
