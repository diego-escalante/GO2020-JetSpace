using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider))]
public class CollisionController : MonoBehaviour {

    public LayerMask solidMask;

    private const float SKIN = 0.015f;
    private const int RAYCAST_COUNT = 5;

    private BoxCollider coll;
    private Vector3 colliderHalfDims;
    private RaycastOrigins raycastOrigins;
    private CollisionInfo collisionInfo;

    public struct CollisionInfo {
        public Dictionary<Vector3, Collider> colliders;
        public Vector3 moveVector;

        public void Reset(Vector2 moveVector) {
            this.moveVector = moveVector;
            if (colliders == null) {
                colliders = new Dictionary<Vector3, Collider>();
            } else {
                colliders.Clear();
            }
        }
    }

    private void Start() {
        coll = GetComponent<BoxCollider>();
        colliderHalfDims = new Vector3(
            coll.size.x / 2 * transform.localScale.x - SKIN,
            coll.size.y / 2 * transform.localScale.y - SKIN,
            coll.size.z / 2 * transform.localScale.z - SKIN);
    }

    public ref CollisionInfo Check(Vector3 moveVector) {
        UpdateRaycastOrigins();
        collisionInfo.Reset(moveVector);

        Collider tempColl;
        Vector3 direction;
        if (moveVector.x != 0) {
            direction = Vector3.right * Mathf.Sign(moveVector.x);
            collisionInfo.moveVector.x = CastRays(moveVector.x, direction, out tempColl);
            if (tempColl != null) {
                collisionInfo.colliders.Add(direction, tempColl);
            }
        }
        if (moveVector.y != 0) {
            direction = Vector3.up * Mathf.Sign(moveVector.y);
            collisionInfo.moveVector.y = CastRays(moveVector.y, direction, out tempColl);
            if (tempColl != null) {
                collisionInfo.colliders.Add(direction, tempColl);
            }
        }
        if (moveVector.z != 0) {
            direction = Vector3.forward * Mathf.Sign(moveVector.z);
            collisionInfo.moveVector.z = CastRays(moveVector.z, direction, out tempColl);
            if (tempColl != null) {
                collisionInfo.colliders.Add(direction, tempColl);
            }
        }

        return ref collisionInfo;
    }

    // Casts a 2D array of rays from a plane perpendicular to the direction of the rays, returns the shortest distance
    // of rays hit or the original distance.
    private float CastRays(float distance, Vector3 direction, out Collider coll) {
        Vector3 start, end;
        float hitDistance = distance;
        coll = null;
        GetStartAndEndOrigins(direction, out start, out end);
        
        // Cast rays from the face of the collider in a grid.
        for(int i = 0; i < RAYCAST_COUNT; i++) {
            for (int j = 0; j < RAYCAST_COUNT; j++) {
                Vector3 origin = calculateOrigin(start, end, direction, i, j);
                RaycastHit hit = RaycastWithDebug(origin, direction, Mathf.Abs(distance) + SKIN, solidMask);
                if (hit.collider != null) {
                    int sign = Vector3Sign(direction);
                    float currentDist = (hit.distance - SKIN) * sign;
                    if (sign == 1 && currentDist < hitDistance) {
                        hitDistance = currentDist;
                        coll = hit.collider;
                    } else if (sign == -1 && currentDist > hitDistance) {
                        hitDistance = currentDist;
                        coll = hit.collider;
                    }
                }
            }
        }
        return Mathf.Min(hitDistance);
    }

    // A wrapper for Physics.Raycast to see them in the inspector.
    private RaycastHit RaycastWithDebug(Vector3 origin, Vector3 direction, float distance, int layerMask) {
        RaycastHit hit;
        bool didHit = Physics.Raycast(origin, direction, out hit, distance, layerMask);
        Color color = didHit ? Color.red : Color.green;
        Debug.DrawRay(origin, direction * distance, color);

        return hit;
    }

    // A helpful struct to keep track of the position of the box's vertices. We actually only need half of the points, so long as they are not adjacent to each other.
    private struct RaycastOrigins {
        public Vector3 _000, _011, _101, _110/*, _001, _010, _100, _111*/;
    }

    private void UpdateRaycastOrigins() {
        Vector3 center = transform.position + coll.center;
        raycastOrigins._000 = center + new Vector3(-colliderHalfDims.x, -colliderHalfDims.y, -colliderHalfDims.z);
        // raycastOrigins._001 = center + new Vector3(-colliderHalfDims.x, -colliderHalfDims.y, colliderHalfDims.z);
        // raycastOrigins._010 = center + new Vector3(-colliderHalfDims.x, colliderHalfDims.y, -colliderHalfDims.z);
        raycastOrigins._011 = center + new Vector3(-colliderHalfDims.x, colliderHalfDims.y, colliderHalfDims.z);
        // raycastOrigins._100 = center + new Vector3(colliderHalfDims.x, -colliderHalfDims.y, -colliderHalfDims.z);
        raycastOrigins._101 = center + new Vector3(colliderHalfDims.x, -colliderHalfDims.y, colliderHalfDims.z);
        raycastOrigins._110 = center + new Vector3(colliderHalfDims.x, colliderHalfDims.y, -colliderHalfDims.z);
        // raycastOrigins._111 = center + new Vector3(colliderHalfDims.x, colliderHalfDims.y, colliderHalfDims.z);
    }

    // Figures out the start and end ray origins for a given direction.
    private void GetStartAndEndOrigins(Vector3 direction, out Vector3 start, out Vector3 end) {
        if (direction == Vector3.up) {
            start = raycastOrigins._011;
            end = raycastOrigins._110;
        } else if (direction == Vector3.down) {
            start = raycastOrigins._000;
            end = raycastOrigins._101;
        } else if (direction == Vector3.left) {
            start = raycastOrigins._000;
            end = raycastOrigins._011;
        } else if (direction == Vector3.right) {
            start = raycastOrigins._101;
            end = raycastOrigins._110;
        } else if (direction == Vector3.forward) {
            start = raycastOrigins._011;
            end = raycastOrigins._101;
        } else if (direction == Vector3.back) {
            start = raycastOrigins._000;
            end = raycastOrigins._110;
        } else {
            Debug.LogError("Direction " + direction + " is not aligned in any of the XYZ axes.");
            start = raycastOrigins._000;
            end = raycastOrigins._000;
        }
    }

    // Calculates the origin of a ray in its two dimensional array based on the start/end rays and the direction.
    private Vector3 calculateOrigin(Vector3 start, Vector3 end, Vector3 direction, int i, int j) {
        Vector3 origin;

        if (direction == Vector3.up || direction == Vector3.down) {
            origin.y = start.y;
            origin.x = Mathf.Lerp(start.x, end.x, (float)i/(RAYCAST_COUNT-1));
            origin.z = Mathf.Lerp(start.z, end.z, (float)j/(RAYCAST_COUNT-1));
        } else if (direction == Vector3.left || direction == Vector3.right) {
            origin.x = start.x;
            origin.y = Mathf.Lerp(start.y, end.y, (float)i/(RAYCAST_COUNT-1));
            origin.z = Mathf.Lerp(start.z, end.z, (float)j/(RAYCAST_COUNT-1));
        } else if (direction == Vector3.forward || direction == Vector3.back) {
            origin.z = start.z;
            origin.x = Mathf.Lerp(start.x, end.x, (float)i/(RAYCAST_COUNT-1));
            origin.y = Mathf.Lerp(start.y, end.y, (float)j/(RAYCAST_COUNT-1));
        } else {
            Debug.LogError("Direction " + direction + " is not aligned in any of the XYZ axes.");
            origin = Vector3.zero;
        }

        return origin;
    }

    private int Vector3Sign(Vector3 v) {
        if (v == Vector3.up || v == Vector3.right || v == Vector3.forward) {
            return 1;
        } else if (v == Vector3.down || v == Vector3.left || v == Vector3.back) {
            return -1;
        } else {
            Debug.LogError("Vector " + v + " is not aligned in any of the XYZ axes.");
            return 0;
        }
    }

}
