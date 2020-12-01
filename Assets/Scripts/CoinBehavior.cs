using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CoinBehavior : MonoBehaviour, IPlayerCollidable {
    
    public LayerMask solidMask;

    private Color coinColor;
    private Animator animator;
    private BoxCollider coll, playerColl;

    private static CoinManager coinManager;

    private void OnEnable() {
        coll = GetComponent<BoxCollider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        // Position coin correctly depending on whether the coin is above ground or not.
        RaycastHit hit = Helpers.RaycastWithDebug(transform.position, Vector3.down, 3.1f, solidMask);
        transform.Translate(Vector3.down * (hit.collider == null ? 1 : 2));

        // Start the floating animation at a random point.
        GetComponent<Animator>().Play("Float", 0, Random.Range(0, 1f));

        coinColor = transform.Find("Sprite").GetComponent<SpriteRenderer>().color;
        playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();

        coinManager = playerColl.GetComponent<CoinManager>();
    }

    public void Collided(Vector3 v) {
        coinManager.AddCoin(coinColor);
        playerColl.GetComponent<SoundController>().PlayCoinSound();
        Destroy(gameObject);
    }
}
