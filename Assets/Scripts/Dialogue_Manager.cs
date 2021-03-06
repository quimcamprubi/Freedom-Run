using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Manager : MonoBehaviour
{
    public Dialogue dialogue;

    public GameObject dialoguePanel;
    public TextMeshProUGUI displayText;
    public bool pauseAndUnpause;
    public float typingSpeed;

    private string activeSentence;

    private Queue<string> sentences;

    private void Start()
    {
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Dialog")) DisplayNextSentence();
    }


    private void OnTriggerEnter2D(Collider2D colliderEn)
    {
        if (colliderEn.CompareTag("Player"))
        {
            dialoguePanel.SetActive(true);
            if (pauseAndUnpause)
            {
                var gwendolineObject = GameObject.Find("Gwendoline Boss");
                var griseldaObject = GameObject.Find("Griselda");
                var guards = GameObject.FindGameObjectsWithTag("Enemy");
                var stairway = GameObject.Find("StairwayToPorron");
                if (gwendolineObject != null) gwendolineObject.GetComponent<Gwendoline>().enabled = false;
                if (griseldaObject != null)
                {
                    griseldaObject.GetComponent<PlayerController>().enabled = false;
                    var attackMode = griseldaObject.GetComponent<PlayerAttack>();
                    if (attackMode != null) attackMode.enabled = false;
                }

                if (guards.Length != 0)
                    foreach (var t in guards)
                        t.GetComponent<AIPatrol>().enabled = false;
                if (stairway != null) stairway.gameObject.GetComponentInChildren<StairwayToPorron>().enabled = false;
            }

            StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D colliderEx)
    {
        if (colliderEx.CompareTag("Player"))
        {
            dialoguePanel.SetActive(false);
            if (pauseAndUnpause)
            {
                var gwendolineObject = GameObject.Find("Gwendoline Boss");
                var griseldaObject = GameObject.Find("Griselda");
                var guards = GameObject.FindGameObjectsWithTag("Enemy");
                var stairway = GameObject.Find("StairwayToPorron");
                if (gwendolineObject != null) gwendolineObject.GetComponent<Gwendoline>().enabled = true;
                if (griseldaObject != null)
                {
                    griseldaObject.GetComponent<PlayerController>().enabled = true;
                    var attackMode = griseldaObject.GetComponent<PlayerAttack>();
                    if (attackMode != null) attackMode.enabled = true;
                }

                if (guards.Length != 0)
                    foreach (var t in guards)
                        t.GetComponent<AIPatrol>().enabled = true;
                if (stairway != null) stairway.gameObject.GetComponentInChildren<StairwayToPorron>().enabled = true;
            }

            StopAllCoroutines();
        }
    }

    private void StartDialogue()
    {
        sentences.Clear();

        foreach (var sentence in dialogue.sentenceList) sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (sentences.Count <= 0)
        {
            dialoguePanel.SetActive(false);
            if (pauseAndUnpause)
            {
                var gwendolineObject = GameObject.Find("Gwendoline Boss");
                var griseldaObject = GameObject.Find("Griselda");
                var guards = GameObject.FindGameObjectsWithTag("Enemy");
                var stairway = GameObject.Find("StairwayToPorron");
                if (gwendolineObject != null) gwendolineObject.GetComponent<Gwendoline>().enabled = true;
                if (griseldaObject != null)
                {
                    griseldaObject.GetComponent<PlayerController>().enabled = true;
                    var attackMode = griseldaObject.GetComponent<PlayerAttack>();
                    if (attackMode != null) attackMode.enabled = true;
                }

                if (guards.Length != 0)
                    foreach (var t in guards)
                        t.GetComponent<AIPatrol>().enabled = true;
                if (stairway != null) stairway.gameObject.GetComponentInChildren<StairwayToPorron>().enabled = true;
            }

            StopAllCoroutines();
            return;
        }

        activeSentence = sentences.Dequeue();
        displayText.text = activeSentence;

        StopAllCoroutines();
        StartCoroutine(TypeTheSentence(activeSentence));
    }

    private IEnumerator TypeTheSentence(string sentence)
    {
        displayText.text = "";

        foreach (var letter in sentence.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}