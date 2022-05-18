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
        if (Input.GetKeyDown(KeyCode.Return)) DisplayNextSentence();
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
                gwendolineObject.GetComponent<Gwendoline>().enabled = false;
                griseldaObject.GetComponent<PlayerController>().enabled = false;
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
                var griseldaObject = GameObject.Find("Griselda");
                var gwendolineObject = GameObject.Find("Gwendoline Boss");
                griseldaObject.GetComponent<PlayerController>().enabled = true;
                gwendolineObject.GetComponent<Gwendoline>().enabled = true;
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
                gwendolineObject.GetComponent<Gwendoline>().enabled = true;
                griseldaObject.GetComponent<PlayerController>().enabled = true;
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