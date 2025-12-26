using UnityEngine;
using UnityEngine.EventSystems;

public class ClickImageTrigger : MonoBehaviour, IPointerClickHandler
{
    [Header("要显示的 Panel")]
    public GameObject dialoguePanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue Panel 没有拖入！");
            return;
        }

        // 🔁 取反当前显示状态
        bool isActive = dialoguePanel.activeSelf;
        dialoguePanel.SetActive(!isActive);

        Debug.Log("Panel 状态切换为：" + (!isActive));
    }
}