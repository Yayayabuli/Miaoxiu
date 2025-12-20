using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;

public class UI_Manager : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject successUI;
    public GameObject unfinishedUI;
    public GameObject partialErrorUI;
    public GameObject menuUI;

    [Header("引用")]
    public MouseInteraction mouseInteraction;
    
    [Header("第三关成功反馈")]
    public Level3SuccessDialogue level3SuccessDialogue;

    /// <summary>
    /// 1️⃣ 检查完成情况按钮点击
    /// 遍历所有可交互图片，检查缩放和位置是否正确
    /// </summary>
    public void OnCheckCompletion()
    {
        bool hasError = false;

        var allChecks = MapManager.Instance.GetAllCheckScripts();

        Debug.Log("Check count = " + allChecks.Length);

        foreach (var c in allChecks)
        {
            // 第三关：只检查可交互拼图
            if (MapManager.Instance.canDrag && !c.isInteractable)
                continue;

            if (!c.CheckNow())
            {
                hasError = true;
                break;
            }
        }

        if (!hasError)
        {
            ShowSuccess();
        }
        else if (MapManager.Instance.canDrag)
        {
            ShowPartialError();
        }
        else
        {
            ShowUnfinished();
        }
    }

    /// <summary>
    /// 2️⃣ 重置当前关卡按钮点击
    /// 重置拼图位置和缩放
    /// </summary>
    public void OnResetLevel()
    {
        foreach (var piece in mouseInteraction.allPieces)
        {
            if (!piece.isInteractable) continue;

            // 重置缩放
            RectTransform rt = piece.GetComponent<RectTransform>();
            rt.localScale = Vector3.one * piece.initialScale;

            // 重置位置
            Vector2 originalPos = MapManager.Instance.GetOriginalPosition(piece);
            rt.anchoredPosition = originalPos;
        }

        // 如果 MouseInteraction 内部有状态，也可以重置
        mouseInteraction.ResetDraggingState();

        // 隐藏所有UI
        successUI.SetActive(false);
        unfinishedUI.SetActive(false);
        partialErrorUI.SetActive(false);
    }

    /// <summary>
    /// 3️⃣ 菜单按钮点击
    /// 打开菜单UI
    /// </summary>
    public void OnOpenMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(true);
        }
    }

    // --------------- 保留原有函数 ---------------
    public void ShowSuccess()
    {
        successUI.SetActive(true);

        if (level3SuccessDialogue != null)
        {
            level3SuccessDialogue.PlaySuccessDialogue();
        }
    }


    public void ShowUnfinished()
    {
        unfinishedUI.SetActive(true);
    }

    public void ShowPartialError()
    {
        partialErrorUI.SetActive(true);
    }

    public void CloseAllPanels()
    {
        if (successUI != null)
            successUI.SetActive(false);

        if (unfinishedUI != null)
            unfinishedUI.SetActive(false);

        if (partialErrorUI != null)
            partialErrorUI.SetActive(false);

        if (menuUI != null)
            menuUI.SetActive(false);
    }

}
