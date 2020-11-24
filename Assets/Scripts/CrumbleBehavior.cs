using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleBehavior : MonoBehaviour, IPlayerCollidable {

    public Color startColor, endColor;
    public float timeToCrumble = 2f;
    public int crumbleSteps = 4;
    public float timeToRespawn = 4f;

    private Coroutine crumbling;
    private Material mat;
    private Color originalColor;
    private Collider coll;

    private ObjectShaker shaker;
    private Animator animator;

    private void OnEnable() {
        coll = GetComponent<Collider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        shaker = transform.Find("MatParent/Material").GetComponent<ObjectShaker>();
        mat = shaker.GetComponent<Renderer>().material;
        originalColor = mat.GetColor("_Color");
        animator = transform.parent.GetComponent<Animator>();
    }

    public void Collided(Vector3 collisionDirection) {
        // if collision is not from above or already crumbling, ignore.
        if (collisionDirection != Vector3.up || crumbling != null) {
            return;
        }

        crumbling = StartCoroutine(Crumble());
    }

    private IEnumerator Crumble() {
        float timeStep = timeToCrumble/crumbleSteps;

        for (int i = 1; i <= crumbleSteps; i++) {
            shaker.Shake(10, 0.15f, 0.01f, true, Vector2.zero);
            mat.SetColor("_Color", Color.Lerp(startColor, endColor, i/(float)crumbleSteps));
            yield return new WaitForSeconds(timeStep);
        }

        coll.enabled = false;
        animator.SetBool("Crumbled", true);
        yield return new WaitForSeconds(timeToRespawn);

        animator.SetBool("Crumbled", false);
        shaker.Shake(10, 0.15f, 0.01f, true, Vector2.zero);
        mat.SetColor("_Color", originalColor);
        coll.enabled = true;
        crumbling = null;
    }

}
