using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject dialoguePanel;

    // ❌ 不再使用文字
    // public Text dialogueText;

    [Header("语音")]
    public AudioSource audioSource;

    [Header("逐字显示速度（已无效）")]
    public float typeSpeed = 0.05f;

    [Header("点击触发设置")]
    [Tooltip("需要点击多少次后，才开始显示第一句")]
    public int clickToStart = 1;

    // ❌ 不再使用文字内容
    // private string[] lines;
    private AudioClip[] voices;

    private int index = -1;

    // ❌ 不再使用打字状态
    // private bool isTyping = false;

    private bool dialogueActive = false;

    private int clickCount = 0;
    private bool dialogueStarted = false;

    public void StartDialogue(
        string characterName,
        Sprite portrait,
        string[] dialogueLines,   // ⚠️ 参数保留，但内部不使用
        AudioClip[] voiceClips
    )
    {
        dialogueActive = true;
        dialoguePanel.SetActive(true);

        // ❌ 不再保存文字
        // lines = dialogueLines;
        voices = voiceClips;

        index = -1;

        // ❌ 不再清空文字
        // dialogueText.text = "";

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
            // 🚦还没到开始播放语音的点击次数
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

            // 🎧 现在只有“播放下一句语音”
            NextLine();
        }
    }

    void NextLine()
    {
        index++;
        Debug.Log("NextLine index = " + index);

        if (voices != null && index < voices.Length)
        {
            PlayVoice();
        }
        else
        {
            EndDialogue();
        }
    }

    void PlayVoice()
    {
        audioSource.Stop();

        if (voices[index] != null)
        {
            audioSource.clip = voices[index];
            audioSource.Play();
        }
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);

        audioSource.Stop();

        // 重置
        clickCount = 0;
        dialogueStarted = false;
    }
}
