using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBehavior : MonoBehaviour {

    public float toggleTime = 1.5f;
    public float offsetTime = 0;
    public bool isSolid = true;

    private float currentTime;
    private Renderer rend;
    private ObjectShaker shaker;
    private Collider coll;
    private AudioSource audioSource;
    private static AudioClip toggleClip;

    private void Start() {
        currentTime = offsetTime;
        shaker = transform.Find("Material").GetComponent<ObjectShaker>();
        rend = shaker.GetComponent<Renderer>();
        coll = transform.Find("Collider").GetComponent<Collider>();
        audioSource = gameObject.AddComponent<AudioSource>();
        if (toggleClip == null) {
            toggleClip = GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().toggle;
        }
        audioSource.loop = false;
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = 12;
        audioSource.minDistance = 2.5f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.clip = toggleClip;
    }

    private void Update() {
        currentTime += Time.deltaTime;

        if (currentTime >= toggleTime - 0.15f) {
            //This first shake is a hack and can be better but oh no look at the time.
            shaker.Shake(10, 0.15f, 0.01f, true, Vector2.zero, false);
            if (currentTime >= toggleTime) {
                // Don't reset to 0, subtract toggleTime instead. Otherwise we'll eventually drift in timing.
                currentTime -= toggleTime;
                isSolid = !isSolid;
                rend.enabled = isSolid;
                coll.enabled = isSolid;
                audioSource.Play();
                shaker.Shake(10, 0.15f, 0.01f, true, Vector2.zero, false);
            }
        }

    }

}
