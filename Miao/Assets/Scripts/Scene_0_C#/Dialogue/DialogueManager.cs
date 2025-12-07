using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject dialoguePanel;
    public GameObject namePanel;
    public Text nameText;
    public Text dialogueText;
    public Image portraitImage;
    

    /*[Header("语音")]
    public AudioSource audioSource;*/

    [Header("逐字显示速度")]
    public float typeSpeed = 0.05f;

    private string[] lines;
    private int index;
    private bool isTyping;
    private bool dialogueActive;

    public void StartDialogue(string characterName, Sprite portrait, string[] dialogueLines, AudioClip[] voiceClips)
    {
        dialogueActive = true;
        dialoguePanel.SetActive(true);
        namePanel.SetActive(true);

        nameText.text = characterName;
        portraitImage.sprite = portrait;

        lines = dialogueLines;
        index = 0;

        StopAllCoroutines();
        StartCoroutine(TypeLine(voiceClips));
    }

    IEnumerator TypeLine(AudioClip[] voices)
    {
        isTyping = true;
        dialogueText.text = "";

        string currentLine = lines[index];
        /*
        if (voices != null && index < voices.Length && voices[index] != null)
        {
            audioSource.clip = voices[index];
            audioSource.Play();
        }
        */

        for (int i = 0; i < currentLine.Length; i++)
        {
            dialogueText.text += currentLine[i];
            
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

  
    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                return;
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            StartCoroutine(TypeLine(null));
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        /*audioSource.Stop();*/
    }
}
