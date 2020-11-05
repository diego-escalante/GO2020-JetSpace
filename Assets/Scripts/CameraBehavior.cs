using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

    public LayerMask solidMask;

    public float camAngle = 45f;
    public float topAngle = 70f;
    public float percentPerSecond = 1f;
    public float camDistance = 15f;

    private float currentPercent = 0;
    private float targetPercent = 0;
    private Transform target;
    private Vector3 defaultLocalPosition, defaultLocalForward;
    private PlayerMovement playerMovement;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = target.GetComponent<PlayerMovement>();

        repositionCam(camAngle);
        defaultLocalPosition = transform.localPosition;
        defaultLocalForward = transform.forward;
    }

    void Update() {
        // Only update target camera angle when grounded. 
        // It is too jarring to have the camera shift while jumping.
        if (playerMovement.IsGrounded()) {
            targetPercent = isTargetOccluded() ? 1 : 0;
        }

        // If we are not at the target angle, always work towards getting there.
        // This means do not stop shifting the camera if we stop being grounded
        // while we already started shifting. It's even more jarring to stop
        // mid shift during a jump and continue when grounded again.
        if (targetPercent != currentPercent) {
            if (currentPercent < targetPercent) {
                currentPercent = Mathf.Clamp(currentPercent + percentPerSecond * Time.deltaTime, 0, 1);
                repositionCam(Mathf.SmoothStep(camAngle, topAngle, currentPercent));
            } else if (currentPercent > targetPercent) {
                currentPercent = Mathf.Clamp(currentPercent - percentPerSecond * Time.deltaTime, 0, 1);
                repositionCam(Mathf.SmoothStep(camAngle, topAngle, currentPercent));
            }
        }
    }

    private bool isTargetOccluded() {
        RaycastHit hit = Helpers.RaycastWithDebug(defaultLocalPosition + transform.parent.position, defaultLocalForward, camDistance, solidMask);
        return hit.collider != null;
    }

    private void repositionCam(float angle) {
        transform.localPosition = new Vector3(0, Mathf.Sin(angle*Mathf.Deg2Rad) * camDistance, -Mathf.Cos(angle*Mathf.Deg2Rad) * camDistance);
        transform.LookAt(target);
    }
}
