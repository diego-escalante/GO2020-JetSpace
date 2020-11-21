using System.Collections;
using UnityEngine;

public class ObjectShaker : MonoBehaviour {

    /*
     * ObjectShaker can be used to easily shake game objects by specifying some parameters. This is very useful for
     * things like camera shakes.
     */ 

    // TODO: Right now this script needs to be attached to every object that needs to shake, but you could easily make this a static class that is
    // object agnostic; pass in any transform and it'll shake it up.

    private Coroutine currentCoroutine;
    private Vector3 originalPosition;

    /*
     * Public method that kicks off a set of shakes. If there is currently one running, it will cancel it and restart.
     */
    public void Shake(int amount, float range, float duration, bool decay, Vector2 direction) {
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
            transform.localPosition = originalPosition;
        }
        currentCoroutine = StartCoroutine(ShakeCoroutine(amount, range, duration, decay, direction));
    }

    /*
     * The coroutine that handles the shaking of the game object.
     * amount: The number of shakes.
     * range: How far the shakes go from the origin. (The strength of the shake.)
     * duration: How long each individual shake lasts.
     * decay: If the range of each individual shake diminishes from the previous one.
     * direction: Specifies if the shakes should bounce along a particular direction. (Setting it to a vector of zero will cause the individual direction of each shake to be random.)
     */
    private IEnumerator ShakeCoroutine(int amount, float range, float duration, bool decay, Vector2 direction) {

        // Setup.
        originalPosition = transform.localPosition;
        Vector3 currentPosition;
        direction = direction.normalized;
        Vector3 newPos = new Vector3();
        float elapsedTime;
        float scale = 1;
        int sign = 1;

        // Loop controlling each individual shake.
        // Each loop calculates the appropriate new position of the object based on scale, direction, and range.
        // Then it smoothly moves to that position based on the duration.
        for (int i = 0; i < amount; i++) {
            // If there is a decay, reduce the scale.
            if (decay) scale = (amount - (float)i) / amount;

            // If this is the last shake, return to the original position.
            if (i == amount - 1) {
                newPos = originalPosition;
            }
            // Else if the direction is zero, come up with a random position.
            else if (direction == Vector2.zero) {
                newPos = originalPosition + new Vector3(Random.Range(-range, range) * scale, Random.Range(-range, range) * scale, 0);
            }
            // Else come up with a position based on the direction.
            else {
                newPos = originalPosition + (Vector3)direction * scale * range * sign;
                sign *= -1;
            }

            // Smoothly move to this position.
            elapsedTime = 0f;
            currentPosition = transform.localPosition;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                transform.localPosition = new Vector3(Mathf.SmoothStep(currentPosition.x, newPos.x, elapsedTime / duration), Mathf.SmoothStep(currentPosition.y, newPos.y, elapsedTime / duration), originalPosition.z);
                yield return null;
            }
        }

        //Done!
        currentCoroutine = null;
    }
}