using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenDialogue : MonoBehaviour
{
    public Dialogue dialogue;
    public string sceneDestination;
    public GameObject dialoguePanel;
    public TextMeshProUGUI displayText;
    public float typingSpeed;

    private string activeSentence;

    private Queue<string> sentences;

    private void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(true);
        StartDialogue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) DisplayNextSentence();
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
            StopAllCoroutines();
            SceneManager.LoadScene(sceneDestination);
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