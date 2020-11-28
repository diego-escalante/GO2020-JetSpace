using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour {

    private Queue<Dialogue.SubDialogue> subDialogues = new Queue<Dialogue.SubDialogue>();
    private Animator animator;
    private Coroutine co;
    public bool isOpen = false;
    private bool canContinue = false;

    private TMP_Text uiText;
    private Image leftProfile;
    private Image rightProfile;
    private GameObject buttonPrompt;

    private Color leftProfileColor;
    private Color rightProfileColor;
    private Color noProfileColor;
    private float defaultFontSize;
    private float defaultSpeed;

    private TollDialogue tollDialogue;

    private void Start() {
        Transform trans = GameObject.FindWithTag("UI").transform.Find("Dialogue Box");
        uiText = trans.Find("HList/Text").GetComponent<TMP_Text>();
        leftProfile = trans.Find("HList/Left Profile").GetComponent<Image>();
        rightProfile = trans.Find("HList/Right Profile").GetComponent<Image>();
        buttonPrompt = trans.Find("Button").gameObject;

        animator = trans.GetComponent<Animator>();
        animator.SetBool("isOpen", isOpen);
    }

    public void Update() {
        if (isOpen && canContinue && Input.GetKeyDown(KeyCode.Space)) {
            DisplayNextText();
        }
    }

    public void StartDialogue(TollDialogue tollDialogue, Dialogue dialogue) {
        this.tollDialogue = tollDialogue;
        subDialogues.Clear();
        isOpen = true;
        animator.SetBool("isOpen", isOpen);

        defaultFontSize = dialogue.defaultFontSize;
        defaultSpeed = dialogue.defaultSpeed;
        leftProfileColor = dialogue.leftProfileColor;
        leftProfile.color = leftProfileColor;
        rightProfileColor = dialogue.rightProfileColor;
        rightProfile.color = rightProfileColor;
        noProfileColor = dialogue.noProfileColor;

        foreach (Dialogue.SubDialogue subDialogue in dialogue.subDialogues) {
            subDialogues.Enqueue(subDialogue);
        }
        DisplayNextText();
    }

    public void DisplayNextText() {
        canContinue = false;
        buttonPrompt.SetActive(false);
        if (subDialogues.Count == 0) {
            EndDialogue();
            return;
        }

        Dialogue.SubDialogue subDialogue = subDialogues.Dequeue();
        uiText.fontSize = subDialogue.fontSize == 0 ? defaultFontSize : subDialogue.fontSize;
        switch (subDialogue.profileLocation) {
            case Dialogue.ProfileLocation.Left:
                uiText.color = leftProfileColor;
                uiText.fontStyle = FontStyles.Normal;
                leftProfile.gameObject.SetActive(true);
                rightProfile.gameObject.SetActive(false);
                break;
            case Dialogue.ProfileLocation.Right:
                uiText.color = rightProfileColor;
                uiText.fontStyle = FontStyles.Normal;
                leftProfile.gameObject.SetActive(false);
                rightProfile.gameObject.SetActive(true);
                break;
            case Dialogue.ProfileLocation.None:
                uiText.color = noProfileColor;
                uiText.fontStyle = FontStyles.Italic; 
                leftProfile.gameObject.SetActive(false);
                rightProfile.gameObject.SetActive(false);
                break;
        }
        if (co != null) {
            StopCoroutine(co);
        }
        co = StartCoroutine(ScrollCharacters(subDialogue.text, subDialogue.textSpeed <= 0 ? defaultSpeed : subDialogue.textSpeed));

    }

    private void EndDialogue() {
        Time.timeScale = 1f;
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
        // tollDialogue.Collided(Vector3.zero);
    }

    private IEnumerator ScrollCharacters(string text, float speed) {
        bool inBrackets = false;
        for(int i = 0; i < text.Length; i++) {
            if (text[i] == '<') {
                inBrackets = true;
            } else if (text[i] == '>') {
                inBrackets = false;
            } 

            if (inBrackets) {
                continue;
            }

            string revealedText = text.Substring(0, i+1);
            string hiddenText = "<color=#00000000>" + Regex.Replace(text.Substring(i+1), @"<sprite=\d+>", "<sprite=15>") + "</color>";
            uiText.text = revealedText + hiddenText;
            yield return new WaitForSecondsRealtime(speed);
        }
        buttonPrompt.SetActive(true);
        canContinue = true;
    }

    public bool IsOpen() {
        return isOpen;
    }
}
