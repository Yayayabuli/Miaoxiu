using UnityEngine;

public class ClickImageTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("角色信息")]
    public string characterName = "角色名";
    public Sprite portrait;
    public string[] dialogueLines;
    public AudioClip[] voiceClips;

    private bool hasTriggered = false;

    private void OnMouseDown()
    {
        if (hasTriggered) return;

        if (dialogueManager == null)
        {
            /*Debug.LogError("DialogueManager 没有拖入！");*/
            return;
        }

        dialogueManager.StartDialogue(characterName, portrait, dialogueLines, voiceClips);
        hasTriggered = true;
    }
}