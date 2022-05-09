using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Dialogue_Manager : MonoBehaviour
{
    public Dialogue dialogue;

    Queue<string> sentences;

    public GameObject dialoguePanel;
    public TextMeshProUGUI displayText;
    public bool pauseAndUnpause = false;

    string activeSentence;
    public float typingSpeed;

    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }
    }
    
    void StartDialogue()
    {
        sentences.Clear();

        foreach (string sentence in dialogue.sentenceList)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if(sentences.Count <= 0)
        {
            dialoguePanel.SetActive(false);
            if (pauseAndUnpause) {
                GameObject gwendolineObject = GameObject.Find("Gwendoline Boss");
                GameObject griseldaObject = GameObject.Find("Griselda");
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

    IEnumerator TypeTheSentence(string sentence)
    {
        displayText.text = "";

        foreach(char letter in sentence.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    private void OnTriggerEnter2D(Collider2D colliderEn)
    {
        if(colliderEn.CompareTag("Player"))
        {
            dialoguePanel.SetActive(true);
            if (pauseAndUnpause) {
                GameObject gwendolineObject = GameObject.Find("Gwendoline Boss");
                GameObject griseldaObject = GameObject.Find("Griselda");
                gwendolineObject.GetComponent<Gwendoline>().enabled = false;
                griseldaObject.GetComponent<PlayerController>().enabled = false;
            }
            StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D colliderEx)
    {
        if(colliderEx.CompareTag("Player"))
        {
            dialoguePanel.SetActive(false);
            if (pauseAndUnpause) {
                GameObject griseldaObject = GameObject.Find("Griselda");
                GameObject gwendolineObject = GameObject.Find("Gwendoline Boss");
                griseldaObject.GetComponent<PlayerController>().enabled = true;
                gwendolineObject.GetComponent<Gwendoline>().enabled = true;
            }
            StopAllCoroutines();
        }
    }
       


}
