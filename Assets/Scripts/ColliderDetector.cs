using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour {
    
    public LayerMask collectibleMask;
    public LayerMask toDropMask;

    private CollisionController.CollisionInfo collInfo;
    private PlayerMovement playerMovement;
    private CollisionController collisionController;
    private BoxCollider coll;
    private Dictionary<Collider, IPlayerCollidable> collidableMap = new Dictionary<Collider, IPlayerCollidable>();

    private Vector3 lastPosition; 
    private float timeElapsed = 0;
    private float timeBetweenChecks = 0.1f;

    private Collider currentTroll;
    private Collider currentStandaloneTroll;

    private void Start() {
        playerMovement = GetComponent<PlayerMovement>();
        collisionController = GetComponent<CollisionController>();
        coll = GetComponent<BoxCollider>();
        timeElapsed = timeBetweenChecks;
    }

    // Trigger collision behaviors from anything with it.
    private void Update() {
        TriggerSolidCollidables();
        TriggerCollectibleCollidables();
        TriggerDroppingTerrain();
        lastPosition = transform.position;
    }

    private void TriggerSolidCollidables() {
        // Get solid collidables from player movement and collisions.
        collInfo = playerMovement.GetCollisionInfo();

        foreach (KeyValuePair<Vector3, Collider> pair in collInfo.colliders) {
            if (!collidableMap.ContainsKey(pair.Value)) {
                continue;
            }

            // Send collided signal. NB: the collision direction is inverted since the perspective changes to the other object.
            collidableMap[pair.Value].Collided(pair.Key * -1);
        }
    }

    // These names are borderline ridiculous.
    private void TriggerCollectibleCollidables() {
        // No need to do this if we haven't moved since last time.
        // if (lastPosition == transform.position) {
        //     return;
        // }

        // Get collectible collidables from special raycasts based on player movement.
        Collider[] collectibleColls = Physics.OverlapBox(transform.position + coll.center, coll.size/2, Quaternion.identity, collectibleMask);

        bool withTroll = false;
        bool withStandaloneTroll = false;
        foreach (Collider collectibleColl in collectibleColls) {
            if (!collidableMap.ContainsKey(collectibleColl)) {
                continue;
            }
            collidableMap[collectibleColl].Collided(Vector3.zero);
            
            if (collectibleColl.gameObject.tag == "Troll") {
                currentTroll = collectibleColl;
                withTroll = true;
            }

            if (collectibleColl.gameObject.tag == "StandaloneTroll") {
                currentStandaloneTroll = collectibleColl;
                withStandaloneTroll = true;
            }

        }

        if (!withTroll && currentTroll != null) {
            currentTroll.transform.parent.parent.GetComponent<TollDialogue>().Exited();
            currentTroll = null;
        }

        if (!withStandaloneTroll && currentStandaloneTroll != null) {
            currentStandaloneTroll.transform.parent.GetComponent<StandaloneTroll>().Exited();
            currentStandaloneTroll = null;
        }

    }

    private void TriggerDroppingTerrain() {
        timeElapsed += Time.deltaTime;

        // No need to check every frame. Just check periodically.
        if (timeElapsed < timeBetweenChecks) {
            return;
        }

        // No need to do this if we haven't move since last time.
        if (lastPosition == transform.position) {
            return;
        }

        timeElapsed = 0;

        // Get terrain nearby that still needs to drop.
        Collider[] terrainToDrop = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 30, 12, toDropMask);

        foreach (Collider terrain in terrainToDrop) {
            terrain.gameObject.layer = LayerMask.NameToLayer("Solid");
            terrain.transform.parent.Find("Box").GetComponent<TerrainBehavior>().Drop();
        }

    }

    public void Register(Collider coll, IPlayerCollidable playerCollidable) {
        collidableMap[coll] = playerCollidable;
    }

    public void Deregister(Collider coll) {
        collidableMap.Remove(coll);
    }

}
