﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireBehavior : MonoBehaviour, IPlayerCollidable {

    private static CheckpointSystem checkpointSystem;
    private Animator anim;
    private Collider coll;

    private void OnEnable() {
        coll = transform.parent.GetComponent<BoxCollider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        anim = GetComponent<Animator>();
        if (checkpointSystem == null) {
            checkpointSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<CheckpointSystem>();
        }
    }

    public void Collided(Vector3 v) {
        checkpointSystem.UpdateRespawn(this);
        SetState(true);
    }

    public void SetState(bool state) {
        anim.SetBool("IsActive", state);
    }

}
