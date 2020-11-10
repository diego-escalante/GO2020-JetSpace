using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static bool Overlaps(this BoxCollider coll, BoxCollider other) {
        return coll.bounds.Intersects(other.bounds);
    }
}
