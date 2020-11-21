using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCollidable {
    void Collided(Vector3 collisionDirection);
}