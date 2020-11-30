using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StandaloneTroll : MonoBehaviour, IPlayerCollidable {
    
    public Color reqCoinColor;

    public Dialogue dialogue;
    public Dialogue postDialogue;

    private bool firstChat = true;

    private static DialogueManager dialogueManager;

    private GameObject buttonPrompt;
    private BoxCollider coll;
    private SpriteRenderer trollSprite;
    private static Transform playerTrans;

    private void OnEnable() {
        coll = transform.Find("Collider").GetComponent<BoxCollider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        if (dialogueManager == null) {
            dialogueManager = GameObject.FindGameObjectWithTag("Player").GetComponent<DialogueManager>();
        }
        trollSprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        trollSprite.color = reqCoinColor;
        if (playerTrans == null) {
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        }
        buttonPrompt = transform.Find("Sprite/World UI/List/Button").gameObject;
    }

    private void Update() {
        trollSprite.flipX = transform.position.x > playerTrans.position.x;

        if (Input.GetKeyDown(KeyCode.Space) && buttonPrompt.activeSelf && !dialogueManager.isOpen) {
            buttonPrompt.SetActive(false);
            if (firstChat) {
                firstChat = false;
                dialogueManager.StartDialogue(dialogue);
            } else {
                dialogueManager.StartDialogue(postDialogue);
            }
        }
    }

    public void Collided(Vector3 v) {
        if (!dialogueManager.isOpen) {
            buttonPrompt.SetActive(true);
        }
    }

    public void Exited() {
        buttonPrompt.SetActive(false);
    }
}
