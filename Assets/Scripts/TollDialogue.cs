using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TollDialogue : MonoBehaviour, IPlayerCollidable {

    public Color reqCoinColor = Color.red;
    public int reqCoinAmount = 10;
    
    public Dialogue initialDialogue;
    public Dialogue reqNotMetDialogue;
    public Dialogue reqMetDialogue;
    public Dialogue doneDialogue;

    private static DialogueManager dialogueManager;
    private static CoinManager coinManager;

    private GameObject buttonPrompt;
    private GameObject coinReqUI;
    private BoxCollider coll;
    private SpriteRenderer trollSprite;
    private static Transform playerTrans;

    private DialogueState dialogueState = DialogueState.Initial;

    private void OnEnable() {
        coll = transform.Find("Troll/Collider").GetComponent<BoxCollider>();
        ColliderExtensions.RegisterToDetector(this, coll);
    }

    private void OnDisable() {
        ColliderExtensions.DeregisterFromDetector(this, coll);
    }

    private void Start() {
        if (dialogueManager == null) {
            dialogueManager = GameObject.FindGameObjectWithTag("Player").GetComponent<DialogueManager>();
        }
        if (coinManager == null) {
            coinManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinManager>();
        }
        trollSprite = transform.Find("Troll/Sprite").GetComponent<SpriteRenderer>();
        trollSprite.color = reqCoinColor;
        if (playerTrans == null) {
            playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        }
        buttonPrompt = transform.Find("Troll/Sprite/World UI/List/Button").gameObject;
        coinReqUI = transform.Find("Troll/Sprite/World UI/List/Coin Req").gameObject;
    }

    private void Update() {
        trollSprite.flipX = transform.position.x > playerTrans.position.x;

        if (Input.GetKeyDown(KeyCode.Space) && buttonPrompt.activeSelf && !dialogueManager.isOpen) {
            buttonPrompt.SetActive(false);

            switch (dialogueState) {
                case DialogueState.Initial:
                    dialogueManager.StartDialogue(this, initialDialogue);
                    dialogueState = DialogueState.Middle;
                    coinReqUI.SetActive(true);
                    coinReqUI.transform.Find("Image").GetComponent<Image>().color = reqCoinColor;
                    TMP_Text text = coinReqUI.transform.Find("Text").GetComponent<TMP_Text>();
                    text.color = reqCoinColor;
                    text.text = reqCoinAmount.ToString();
                    break;
                case DialogueState.Middle:
                    if (coinManager.hasCoins(reqCoinColor, reqCoinAmount)) {
                        coinManager.payCoins(reqCoinColor, reqCoinAmount);
                        dialogueManager.StartDialogue(this, reqMetDialogue);
                        dialogueState = DialogueState.End;
                        coinReqUI.SetActive(false);
                        StartCoroutine(BastionizeBridge());
                    } else {
                        dialogueManager.StartDialogue(this, reqNotMetDialogue);
                    }
                    break;
                case DialogueState.End:
                    dialogueManager.StartDialogue(this, doneDialogue);
                    break;
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

    private enum DialogueState {
        Initial,
        Middle,
        End
    }

    private IEnumerator BastionizeBridge() {
        Transform bridge = transform.Find("Bridge");
        for (int i = 0; i < bridge.childCount; i++) {
            bridge.GetChild(i).GetComponent<BridgeTerrainBehavior>().Drop();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
