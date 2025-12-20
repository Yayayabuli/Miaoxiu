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

    [Header("点击触发设置")]
    [Tooltip("需要点击多少次后，才开始显示第一句")]
    public int clickToStart = 1;

    private string[] lines;
    private AudioClip[] voices;
    private int index = -1;

    private bool isTyping = false;
    private bool dialogueActive = false;

    private int clickCount = 0;
    private bool dialogueStarted = false;

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

        // 重置点击状态
        clickCount = 0;
        dialogueStarted = false;
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            // 🚦还没到开始对话的点击次数
            if (!dialogueStarted)
            {
                clickCount++;

                if (clickCount >= clickToStart)
                {
                    dialogueStarted = true;
                    NextLine(); // 第一次真正开始
                }

                return;
            }

            // 正常对话流程
            if (isTyping)
            {
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
        isTyping = true;
        dialogueText.text = "";

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

        // 可选：重置
        clickCount = 0;
        dialogueStarted = false;
    }
}
