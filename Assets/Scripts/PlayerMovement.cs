using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CollisionController))]
public class PlayerMovement : MonoBehaviour {

    // Jumping
    [Header("Basic Jumping")]
    [SerializeField]
    private float jumpHeight = 1.25f;
    [SerializeField]
    private float timeToJumpApex = 0.33f;
    private float gravity, maxJumpVelocity;

    // Running
    [Header("Basic Running")]
    [SerializeField]
    private float runSpeed = 10f;
    [SerializeField]
    private float timeToTopSpeed = 0.15f;
    private float acceleration;

    // Terminal Velocity
    [Header("Terminal Velocity")]
    [SerializeField]
    private float terminalVelocityFactor = 3f;
    private float terminalVelocity;

    // Multijumping
    [Header("Multijumping")]
    [SerializeField]
    private int jumps = 1;
    [SerializeField]
    private float multijumpHeight = 0.5f;
    private float maxMultiJumpVelocity;
    private int jumpsLeft;

    // Variable Jumping
    [Header("Variable Jumping")]
    [SerializeField]
    private bool variableJumpEnabled = true;
    [SerializeField]
    private float minJumpHeight = 0.25f;
    [SerializeField]
    private float minMultiJumpHeight = 0.25f;
    private float minJumpVelocity;
    private float minMultiJumpVelocity;
    private bool variableJumpActive = false;

    // Coyote Time
    [Header("Coyote Time")]
    [SerializeField]
    private bool coyoteTimeEnabled = true;
    [SerializeField]
    private float coyoteTime = 0.1f;
    private float coyoteTimeLeft;

    // Jump Buffering
    [Header("Jump Buffering")]
    [SerializeField]
    private float jumpBufferTime = 0.09f;
    private float jumpBufferTimeLeft;


    private Vector3 velocity = Vector3.zero;
    private CollisionController collisionController;
    private CollisionController.CollisionInfo collisionInfo;
    private Vector3 lastGroundedPosition = Vector3.zero;

    private void OnValidate() {
        jumpHeight = Mathf.Max(0, jumpHeight);
        minJumpHeight = Mathf.Clamp(minJumpHeight, 0, jumpHeight);
        timeToJumpApex = Mathf.Max(0.01f, timeToJumpApex);
        terminalVelocityFactor = Mathf.Max(1, terminalVelocityFactor);
        runSpeed = Mathf.Max(0, runSpeed);
        timeToTopSpeed = Mathf.Max(0.01f, timeToTopSpeed);
        coyoteTime = Mathf.Max(0, coyoteTime);
        jumps = Mathf.Max(0, jumps);
        multijumpHeight = Mathf.Max(0, multijumpHeight);
        minMultiJumpHeight = Mathf.Clamp(minMultiJumpHeight, 0, multijumpHeight);
        jumpBufferTime = Mathf.Max(0, jumpBufferTime);
        UpdateKinematics();
    }

    private void Awake() {
        UpdateKinematics();
        collisionController = GetComponent<CollisionController>();

        // Quick hack, need this initialized on the first frame.
        collisionInfo = new CollisionController.CollisionInfo();
        collisionInfo.colliders = new Dictionary<Vector3, Collider>();
    }

    private void Update(){

        if (coyoteTimeEnabled) {
            coyoteTimeLeft -= Time.deltaTime;
            // Get rid of primary jump if no coyote time is left.
            if (jumpsLeft == jumps && coyoteTimeLeft <= 0) {
                jumpsLeft--;
            }
        } else {
            // Get rid of primary jump if not on the ground.
            if (jumpsLeft == jumps && !collisionInfo.colliders.ContainsKey(Vector3.down)) {
                jumpsLeft--;
            }
        }

        // Register jump input ahead of time for buffer and use it if applicable, keep track of time since jump input.
        if (jumpBufferTimeLeft >= 0) {
            jumpBufferTimeLeft -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpBufferTimeLeft = jumpBufferTime;
        }

        // Calculate horizontal movement.
        /*TODO (possibly): hInput is not normalized. Meaning you cover more ground if you are traveling diagonally.
        Mathematically, that is wrong: your top speed shouldn't increase if your heading changes. However,
        in the game it FEELS better if you have acceleration. Which means I have to side with game feel. And it
        bothers me so much. I don't know if I am doing something wrong with the numbers that I haven't noticed.
        It's possible that because the player doesn't face the exact direction it is heading (it can only face the
        four cardinal directions), when the player doesn't visually rotate but slows down in one axis when it is
        normalized. Leading to bad game feel. I don't know.*/ 
        Vector3 hInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))/*.normalized*/;
        velocity.x = (velocity.x > hInput.x * runSpeed) ? Mathf.Max(hInput.x * runSpeed, velocity.x - (acceleration * Time.deltaTime)) 
                                                        : Mathf.Min(velocity.x + (acceleration * Time.deltaTime), hInput.x * runSpeed);
        velocity.z = (velocity.z > hInput.z * runSpeed) ? Mathf.Max(hInput.z * runSpeed, velocity.z - (acceleration * Time.deltaTime)) 
                                                        : Mathf.Min(velocity.z + (acceleration * Time.deltaTime), hInput.z * runSpeed);

        // Always apply gravity.
        velocity.y = Mathf.Clamp(velocity.y + gravity * Time.deltaTime, -terminalVelocity, terminalVelocity);

        // Calculate vertical movement.
        if (jumpBufferTimeLeft >= 0 && jumpsLeft > 0) {
            velocity.y = (jumps == jumpsLeft ? maxJumpVelocity : maxMultiJumpVelocity);
            jumpsLeft--;
            // Just set the jump timer to negative to "consume" input.
            jumpBufferTimeLeft = -1;
            if (variableJumpEnabled) {
                variableJumpActive = true;
            }
        }

        // Shortening jumps by releasing space.
        if (variableJumpEnabled && Input.GetKeyUp(KeyCode.Space) && variableJumpActive) {
            variableJumpActive = false;
            if (jumps == jumpsLeft+1) {
                if (velocity.y > minJumpVelocity) {
                    velocity.y = minJumpVelocity;
                }
            } else {
                if (velocity.y > minMultiJumpVelocity) {
                    velocity.y = minMultiJumpVelocity;
                }
            }
        }

        // Do the collision check.
        collisionInfo = collisionController.Check(velocity * Time.deltaTime);

        // If the ground is below us, stop.
        if (collisionInfo.colliders.ContainsKey(Vector3.down) || collisionInfo.colliders.ContainsKey(Vector3.up)) {
            velocity.y = 0;
            // If collision is below, reset jumping and coyote time (if enabled).
            if (collisionInfo.colliders.ContainsKey(Vector3.down)) {
                if (coyoteTimeEnabled) {
                    coyoteTimeLeft = coyoteTime;
                }
                jumpsLeft = jumps;
                lastGroundedPosition = transform.position;
            }
        }

        // React to horizontal collisions. 
        if (collisionInfo.colliders.ContainsKey(Vector3.right) || collisionInfo.colliders.ContainsKey(Vector3.left)) {
            velocity.x = 0;
        }
        if (collisionInfo.colliders.ContainsKey(Vector3.forward) || collisionInfo.colliders.ContainsKey(Vector3.back)) {
            velocity.z = 0;
        }

        // Move the player.
        transform.Translate(collisionInfo.moveVector);
    }

    private void UpdateKinematics(){
        acceleration = runSpeed / timeToTopSpeed;
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        maxMultiJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * multijumpHeight);
        minMultiJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minMultiJumpHeight);
        terminalVelocity = maxJumpVelocity * terminalVelocityFactor;
    }

    public bool IsGrounded() {
        if (collisionInfo.colliders == null) {
            return false;
        }
        return collisionInfo.colliders.ContainsKey(Vector3.down);
    }

    public int JumpsLeft() {
        return jumpsLeft;
    }

    public Vector3 GetVelocity() {
        return velocity;
    }

    public void AddVelocity(Vector3 delta) {
        velocity += delta;
    }

    public float GetGravity() {
        return gravity;
    }

    public Vector3 GetLastGroundedPosition() {
        return lastGroundedPosition;
    }

    public float GetJumpHeight() {
        return jumpHeight;
    }

    public CollisionController.CollisionInfo GetCollisionInfo() {
        return collisionInfo;
    }
}
