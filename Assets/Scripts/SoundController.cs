using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioClip coinSound;
    public AudioClip jumpSound;
    public AudioClip fuelSound;
    public AudioClip dialogueSound;
    public AudioClip jetpackSound;
    public AudioClip fireSound;

    public AudioClip crumble;
    public AudioClip crumbleLite;
    public AudioClip toggle;
    public AudioClip rocket;

    public AudioSource audioSource;


    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCoinSound() {
        audioSource.PlayOneShot(coinSound, 1);
    }

    public void PlayJumpSound() {
        audioSource.PlayOneShot(jumpSound, 1);
    }

    public void PlayFuelSound() {
        audioSource.PlayOneShot(fuelSound, 1);
    }

    public void PlayDialogueSound() {
        audioSource.PlayOneShot(dialogueSound, 1);
    }

    public void PlayJetpackSound() {
        audioSource.PlayOneShot(jetpackSound);
    }

    public void PlayFireSound() {
        audioSource.PlayOneShot(fireSound);
    }

    public void StopSound() {
        audioSource.Stop();
    }
}