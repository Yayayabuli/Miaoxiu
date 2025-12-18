using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("语音")]
    public AudioSource audioSource;

    [Header("逐字显示速度")]
    public float typeSpeed = 0.05f;

    private string[] lines;
    private AudioClip[] voices;
    private int index = -1;

    private bool isTyping = false;
    private bool dialogueActive = false;

    public void StartDialogue(
        string characterName,
        Sprite portrait,
        string[] dialogueLines,
        AudioClip[] voiceClips
    )
    {
        dialogueActive = true;
        dialoguePanel.SetActive(true);

        lines = dialogueLines;
        voices = voiceClips;

        index = -1;
        dialogueText.text = "";

        audioSource.Stop();
        StopAllCoroutines();

        // ✅ 关键：立刻播放第一句
        NextLine();
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 点击补全文字（语音不中断）
                StopAllCoroutines();
                dialogueText.text = lines[index];
                isTyping = false;
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
        Debug.Log("NextLine index = " + index);

        if (index < lines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        Debug.Log("voices is null? " + (voices == null));
        Debug.Log("voices length = " + (voices == null ? -1 : voices.Length));

        isTyping = true;
        dialogueText.text = "";

        // ▶ 播放当前句语音
        if (voices != null && index < voices.Length && voices[index] != null)
        {
            audioSource.clip = voices[index];
            audioSource.Play();
        }

        string currentLine = lines[index];

        foreach (char c in currentLine)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);

        audioSource.Stop();
        dialogueText.text = "";
    }
}
