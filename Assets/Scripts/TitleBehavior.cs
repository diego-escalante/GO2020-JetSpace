using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleBehavior : MonoBehaviour {

    private RawImage fade;
    private Coroutine co;


    private void Start() {
        fade = GameObject.FindGameObjectWithTag("UI").transform.Find("Fade").GetComponent<RawImage>();
        co = StartCoroutine(FadeIn(1f));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StopCoroutine(co);
            StartCoroutine(FadeOut(1f));
        }
    }

    private IEnumerator FadeIn(float duration) {
        for(float elapsedTime = 0; elapsedTime <= duration; elapsedTime += Time.deltaTime) {
            fade.color = Color.Lerp(Color.black, Color.clear, elapsedTime/duration);
            yield return null;
        }
        fade.color = Color.clear;
    }

    private IEnumerator FadeOut(float duration) {
        Color c = fade.color;
        for(float elapsedTime = 0; elapsedTime <= duration; elapsedTime += Time.deltaTime) {
            fade.color = Color.Lerp(c, Color.black, elapsedTime/duration);
            yield return null;
        }
        
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

}
