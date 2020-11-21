using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBehavior : MonoBehaviour {
    
    private Color[] colors = {new Color(92/(float)255, 0, 1), new Color(77/(float)255, 18/(float)255, 179/(float)255), new Color(91/(float)255, 41/(float)255, 179/(float)255)};

    private float dropDistance = 20f;
    private static Transform player;
    private Vector3 originalScale;
    
    private void Start(){
        transform.Translate(Vector3.up * dropDistance);
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        GetComponent<Renderer>().material.SetColor("_Color", colors[Random.Range(0, colors.Length)]);
    }

    public void Drop() {
        StartCoroutine(Bastionize(transform.position, transform.position - Vector3.up * dropDistance, 1f));
    }

    // Transform a 3D vector to a 2D vector; toss out the Y axis.
    private Vector2 Get2DVector(Vector3 vector) {
        return new Vector2(vector.x, vector.z);
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
