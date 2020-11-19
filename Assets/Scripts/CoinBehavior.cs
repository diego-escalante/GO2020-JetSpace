using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CoinBehavior : MonoBehaviour {
    
    public LayerMask solidMask;

    private Color coinColor;
    private Animator animator;
    private BoxCollider coll, playerColl;

    private static CoinManager coinManager;

    private void Start() {
        // Position coin correctly depending on whether the coin is above ground or not.
        RaycastHit hit = Helpers.RaycastWithDebug(transform.position, Vector3.down, 3.1f, solidMask);
        transform.Translate(Vector3.down * (hit.collider == null ? 1 : 2));

        // Start the floating animation at a random point.
        animator = GetComponent<Animator>();
        animator.Play("Float", 0, Random.Range(0, 1f));

        coinColor = transform.Find("Sprite").GetComponent<SpriteRenderer>().color;
        coll = GetComponent<BoxCollider>();
        playerColl = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>();

        coinManager = playerColl.GetComponent<CoinManager>();
    }

    private void Update() {
        if (coll.Overlaps(playerColl)) {
            coinManager.AddCoin(coinColor);
            Destroy(gameObject);
        }
    }
}
