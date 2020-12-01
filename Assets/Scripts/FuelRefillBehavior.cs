using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelRefillBehavior : MonoBehaviour, IPlayerCollidable {

    public Sprite filledSprite;
    public Sprite emptySprite;
    public GameObject blobShadow;

    public float respawnTime = 5f;
    public float fillAmountPercentage = 1f;
    private BoxCollider coll;
    private PlayerHovering playerHovering;
    private SoundController soundController;
    private SpriteRenderer rend;

    private void OnEnable() {
        coll = GetComponent<BoxCollider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        playerHovering = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHovering>();
        soundController = playerHovering.GetComponent<SoundController>();
        rend = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        transform.Translate(Vector3.down);
        GetComponent<Animator>().Play("Wiggle", 0, Random.Range(0, 1f));
    }

    public void Collided(Vector3 v) {
        if (rend.sprite == filledSprite) {
            soundController.PlayFuelSound();
            StartCoroutine(FuelUp());
        }
    }

    private IEnumerator FuelUp() {
        rend.sprite = emptySprite;
        playerHovering.Refuel(fillAmountPercentage);
        blobShadow.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        blobShadow.SetActive(true);
        rend.sprite = filledSprite;
    }
    
}
