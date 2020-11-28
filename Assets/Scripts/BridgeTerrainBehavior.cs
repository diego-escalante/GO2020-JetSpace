using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTerrainBehavior : MonoBehaviour {
    private float dropDistance = 20f;
    private Vector3 originalScale;

    private void Start(){
        transform.Translate(Vector3.up * dropDistance);
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

     public void Drop() {
        StartCoroutine(Bastionize(transform.position, transform.position - Vector3.up * dropDistance, 1f));
    }

    private IEnumerator Bastionize(Vector3 origin, Vector3 destination, float duration) {
        float elapsed = 0;
        Vector3 position = origin;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            position.y = Mathf.SmoothStep(origin.y, destination.y, elapsed/duration);
            transform.localScale = originalScale * Mathf.Min(elapsed/duration, 1);
            transform.position = position;
            yield return null;
        }
        Destroy(this);
    }

}
