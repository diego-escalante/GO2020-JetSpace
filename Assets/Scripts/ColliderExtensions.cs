using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderExtensions {

    static private ColliderDetector colliderDectector;
    
    public static void RegisterToDetector(this IPlayerCollidable iPlayerCollidable, Collider coll) {
        ColliderDetector collDetector = getColliderDetector();
        if (collDetector == null) {
            Debug.LogError("Could not find ColliderDetector to subscribe to!");
            return;
        }
        collDetector.Register(coll, iPlayerCollidable);
    }

    public static void DeregisterFromDetector(this IPlayerCollidable iPlayerCollidable, Collider coll) {
        ColliderDetector collDetector = getColliderDetector();
        if (collDetector == null) {
            return;
        }
        collDetector.Deregister(coll);
    }

    private static ColliderDetector getColliderDetector() {
        if (colliderDectector == null) {
            GameObject gb = GameObject.FindGameObjectWithTag("Player");
            if (gb == null) {
                return null;
            }
            colliderDectector = gb.GetComponent<ColliderDetector>();
        }
        return colliderDectector;
    }
}
