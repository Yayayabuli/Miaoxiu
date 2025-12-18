using UnityEngine;
using UnityEngine.EventSystems;

public class ClickImageTrigger : MonoBehaviour, IPointerClickHandler
{
    public DialogueManager dialogueManager;

    [Header("角色信息")]
    public string characterName = "角色名";
    public Sprite portrait;
    public string[] dialogueLines;
    public AudioClip[] voiceClips;

    private bool hasTriggered = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasTriggered) return;

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager 没有拖入！");
            return;
        }

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogError("对话内容为空，无法触发对话！");
            return;
        }

        dialogueManager.StartDialogue(characterName, portrait, dialogueLines, voiceClips);
        hasTriggered = true;

        Debug.Log("UI 点击成功，对话已触发！");
    }
}