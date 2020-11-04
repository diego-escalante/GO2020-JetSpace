using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers {
    // A wrapper for Physics.Raycast to see them in the inspector.
    public static RaycastHit RaycastWithDebug(Vector3 origin, Vector3 direction, float distance, int layerMask) {
        RaycastHit hit;
        bool didHit = Physics.Raycast(origin, direction, out hit, distance, layerMask);
        Color color = didHit ? Color.red : Color.green;
        Debug.DrawRay(origin, direction * distance, color);

        return hit;
    }
}
