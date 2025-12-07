using UnityEngine;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject successUI;
    public GameObject unfinishedUI;
    public GameObject partialErrorUI;
    public GameObject menuUI;

    [Header("引用")]
    public MouseInteraction mouseInteraction;

    /// <summary>
    /// 1️⃣ 检查完成情况按钮点击
    /// 遍历所有可交互图片，检查缩放和位置是否正确
    /// </summary>
    public void OnCheckCompletion()
    {
        bool allCorrect = true;

        foreach (var piece in mouseInteraction.allPieces)
        {
            if (!piece.isInteractable) continue;

            // 检查缩放
            if (!piece.CheckNow())
            {
                allCorrect = false;
                break;
            }

            // 检查位置是否接近初始位置
            // 这里假设 MapManager 里有 CheckPosition 函数，也可以自定义
            if (MapManager.Instance != null && !MapManager.Instance.CheckPosition(piece))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            ShowSuccess();
        }
        else
        {
            ShowPartialError(); // 或 ShowUnfinished，看你需求
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
