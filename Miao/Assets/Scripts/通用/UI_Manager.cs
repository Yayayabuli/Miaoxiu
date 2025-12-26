using UnityEngine;
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

    [Header("拼图成功金边")]
    public GlowBorder[] glowBorders;

    [Header("倒计时")]
    public TimerManager timerManager;

    void Start()
    {
        // 监听倒计时结束
        if (timerManager != null)
        {
            timerManager.OnTimeUp += OnTimeUp;
        }
    }

    /// <summary>
    /// 1️⃣ 点击“检测完成”
    /// </summary>
    public void OnCheckCompletion()
    {
        bool hasError = false;
        var allChecks = MapManager.Instance.GetAllCheckScripts();

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
            // ✅ 成功：停表 + 播放成功流程
            if (timerManager != null)
                timerManager.StopTimer();

            PlaySuccessFlow();
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
    /// ⭐ 成功流程：金边 → 成功UI
    /// </summary>
    void PlaySuccessFlow()
    {
        successUI.SetActive(false);

        int finished = 0;
        int total = glowBorders.Length;

        // 防止没有金边时报错
        if (total == 0)
        {
            ShowSuccess();
            return;
        }

        foreach (var glow in glowBorders)
        {
            glow.PlayGlow(() =>
            {
                finished++;
                if (finished >= total)
                {
                    ShowSuccess();
                }
            });
        }
    }

    /// <summary>
    /// 成功 UI
    /// </summary>
    public void ShowSuccess()
    {
        successUI.SetActive(true);

        if (level3SuccessDialogue != null)
        {
            level3SuccessDialogue.PlaySuccessDialogue();
        }
    }

    /// <summary>
    /// 未完成
    /// </summary>
    public void ShowUnfinished()
    {
        unfinishedUI.SetActive(true);
    }

    /// <summary>
    /// 部分错误
    /// </summary>
    public void ShowPartialError()
    {
        partialErrorUI.SetActive(true);
    }

    /// <summary>
    /// ⏰ 倒计时结束回调
    /// </summary>
    void OnTimeUp()
    {
        Debug.Log("⏰ Time Up!");

        // 时间到 → 失败
        if (MapManager.Instance.canDrag)
            ShowPartialError();
        else
            ShowUnfinished();
    }

    /// <summary>
    /// 关闭所有 UI
    /// </summary>
    public void CloseAllPanels()
    {
        if (successUI) successUI.SetActive(false);
        if (unfinishedUI) unfinishedUI.SetActive(false);
        if (partialErrorUI) partialErrorUI.SetActive(false);
        if (menuUI) menuUI.SetActive(false);
    }

    /// <summary>
    /// 重置关卡（如果你之后要接按钮）
    /// </summary>
    public void ResetLevel()
    {
        CloseAllPanels();

        if (timerManager != null)
            timerManager.StartTimer();
    }
}
