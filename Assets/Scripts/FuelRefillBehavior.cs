using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
public class FuelRefillBehavior : MonoBehaviour {

    public float respawnTime = 5f;
    public float fillAmountPercentage = 1f;
    private BoxCollider playerColl, coll;
    private float respawnTimeLeft;
    private MeshRenderer rend;

    private void Start() {
        playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();
        coll = GetComponent<BoxCollider>();
        rend = GetComponent<MeshRenderer>();
    }

    private void Update() {
        if (rend.enabled && coll.Overlaps(playerColl)) {
            // If available and touched by player, refill their fuel, disable.
            playerColl.GetComponent<PlayerHovering>().Refuel(fillAmountPercentage);
            rend.enabled = false;
            respawnTimeLeft = respawnTime;
        } else if (!rend.enabled) {
            // If not enabled, tick down and respawn when ready.
            respawnTimeLeft -= Time.deltaTime;
            if (respawnTimeLeft <= 0) {
                rend.enabled = true;
            }
        }
    }
    
}
