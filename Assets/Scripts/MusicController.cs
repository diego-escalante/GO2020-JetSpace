using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

    public AudioSource track1, track2, track3, track4, track5;

    public float volume = 0.5f;
    private int trackLayer = 0;

    private void Start() {
        AddTrack();
    }

    private IEnumerator fadeIn(AudioSource audio, float toVolume, float duration=0.15f) {
        float timeLeft = duration;
        float startingVolume = audio.volume;

        while (timeLeft > 0) {
            timeLeft -= Time.unscaledDeltaTime;
            audio.volume = Mathf.Lerp(startingVolume, toVolume, 1f - (timeLeft/duration));
            yield return null;
        }
        audio.volume = toVolume;
    }

    public void AddTrack() {
        trackLayer++;
        switch (trackLayer) {
            case 1:
            Track1();
            break;
            case 2:
            Track2();
            break;
            case 3:
            Track3();
            break;
            case 4:
            Track4();
            break;
            case 5:
            Track5();
            break;
        }
    }

    // Don't ever do the following below.

    private void Track1() {
        StartCoroutine(fadeIn(track1, volume));
        StartCoroutine(fadeIn(track2, 0));
        StartCoroutine(fadeIn(track3, 0));
        StartCoroutine(fadeIn(track4, 0));
        StartCoroutine(fadeIn(track5, 0));
    }

    private void Track2() {
        StartCoroutine(fadeIn(track1, 0));
        StartCoroutine(fadeIn(track2, volume));
        StartCoroutine(fadeIn(track3, 0));
        StartCoroutine(fadeIn(track4, 0));
        StartCoroutine(fadeIn(track5, 0));
    }

    private void Track3() {
        StartCoroutine(fadeIn(track1, 0));
        StartCoroutine(fadeIn(track2, 0));
        StartCoroutine(fadeIn(track3, volume));
        StartCoroutine(fadeIn(track4, 0));
        StartCoroutine(fadeIn(track5, 0));
    }

    private void Track4() {
        StartCoroutine(fadeIn(track1, 0));
        StartCoroutine(fadeIn(track2, 0));
        StartCoroutine(fadeIn(track3, 0));
        StartCoroutine(fadeIn(track4, volume));
        StartCoroutine(fadeIn(track5, 0));
    }

    private void Track5() {
        StartCoroutine(fadeIn(track1, 0));
        StartCoroutine(fadeIn(track2, 0));
        StartCoroutine(fadeIn(track3, 0));
        StartCoroutine(fadeIn(track4, 0));
        StartCoroutine(fadeIn(track5, volume));
    }

    public void FadeAll() {
        StartCoroutine(fadeIn(track1, 0, 3));
        StartCoroutine(fadeIn(track2, 0, 3));
        StartCoroutine(fadeIn(track3, 0, 3));
        StartCoroutine(fadeIn(track4, 0, 3));
        StartCoroutine(fadeIn(track5, 0, 3));
    }
}