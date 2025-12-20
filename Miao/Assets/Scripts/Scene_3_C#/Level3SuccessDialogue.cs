using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Level3SuccessDialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;        // 成功后显示的文字面板
    public Text dialogueText;

    [Header("语音")]
    public AudioSource audioSource;
    public AudioClip voiceClip;

    [Header("文字内容")]
    [TextArea(2, 5)]
    public string[] lines;

    [Header("打字速度")]
    public float typeSpeed = 0.05f;

    private int index = 0;
    private bool isTyping = false;
    private bool isActive = false;

    /* =======================
     * 对外调用：第三关成功
     * ======================= */
    public void PlaySuccessDialogue()
    {
        Debug.Log("✅ Level3SuccessDialogue 被调用了");
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Level3SuccessDialogue: 没有设置文字内容");
            return;
        }

        panel.SetActive(true);
        dialogueText.text = "";

        index = 0;
        isActive = true;

        StopAllCoroutines();
        StartCoroutine(TypeLine());

        if (voiceClip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(voiceClip);
        }
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
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

        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        isActive = false;
        panel.SetActive(false);
    }
}
