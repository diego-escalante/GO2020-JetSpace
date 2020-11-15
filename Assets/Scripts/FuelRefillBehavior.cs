using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FuelRefillBehavior : MonoBehaviour {

    public Sprite filledSprite;
    public Sprite emptySprite;
    public GameObject blobShadow;

    public float respawnTime = 5f;
    public float fillAmountPercentage = 1f;
    private BoxCollider playerColl, coll;
    private float respawnTimeLeft;
    private SpriteRenderer rend;

    private void Start() {
        playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();
        coll = GetComponent<BoxCollider>();
        rend = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (rend.sprite == filledSprite && coll.Overlaps(playerColl)) {
            // If available and touched by player, refill their fuel, disable.
            playerColl.GetComponent<PlayerHovering>().Refuel(fillAmountPercentage);
            rend.sprite = emptySprite;
            blobShadow.SetActive(false);
            respawnTimeLeft = respawnTime;
        } else if (rend.sprite == emptySprite) {
            // If not enabled, tick down and respawn when ready.
            respawnTimeLeft -= Time.deltaTime;
            if (respawnTimeLeft <= 0) {
                rend.sprite = filledSprite;
                blobShadow.SetActive(true);
            }
        }
    }
    
}
