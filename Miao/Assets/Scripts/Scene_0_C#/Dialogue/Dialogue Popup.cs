using UnityEngine;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    [Header("拖入对话面板")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("拖入面板里的文本组件")]
    [SerializeField] private Text dialogueText; 

    [Header("对话内容")]
    [Multiline(3)]
    [SerializeField] private string message = "滑动鼠标滚轮将样图放大缩小，试着将纹路 “描画” 到合适大小吧";

    private bool waitForClose = true;

    private void Start()
    {
        ShowDialogue();
    }

    private void Update()
    {

        if (waitForClose && (Input.GetMouseButtonDown(0)))
        {
            CloseDialogue();
        }
    }

    private void ShowDialogue()
    {
        if (dialoguePanel == null || dialogueText == null)
        {
            /*Debug.LogError("[DialoguePopup] 面板或文本组件没拖引用！");*/
            return;
        }

        dialogueText.text = message;
        dialoguePanel.SetActive(true);
        waitForClose = true;
    }

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        waitForClose = false;


        Time.timeScale = 1f;
    }
}