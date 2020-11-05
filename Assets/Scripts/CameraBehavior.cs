using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

    public LayerMask solidMask;

    public float camAngle = 45f;
    public float topAngle = 70f;
    public float percentPerSecond = 1f;
    public float camDistance = 15f;

    private float currentPercent;
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
        if (playerMovement.IsGrounded()) {
            if (isTargetOccluded() && currentPercent < 1) {
                currentPercent = Mathf.Clamp(currentPercent + percentPerSecond * Time.deltaTime, 0, 1);
                repositionCam(Mathf.SmoothStep(camAngle, topAngle, currentPercent));
            } else if (currentPercent > 0) {
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
