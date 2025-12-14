using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Header("这关能拖拽吗")]
    public bool canDrag = false;

    [Header("关卡大小（2/3/4）")]
    public int gridSize = 3;

    [Header("拼图槽位父物体")]
    public RectTransform slotsParent;

    [Header("拼图图片数组（4/9/16）")]
    public PuzzleData[] puzzleDatas;

    [Header("鼠标交互脚本引用")]
    public MouseInteraction mouseInteraction;

    [Header("CheckCorrect 脚本引用会自动读取")]
    public CheckCorrect[] checkScripts;

    [Header("UI 管理脚本")]
    public UI_Manager uiManager;

    [Header("第三关可交互图片索引")]
    public List<int> interactIndexes = new List<int>();

    [Header("位置容差（像素）")]
    public float positionTolerance = 10f;

    public static MapManager Instance;

    // 保存每张拼图初始位置
    private Dictionary<CheckCorrect, Vector2> originalPositions = new Dictionary<CheckCorrect, Vector2>();

    private void Awake()
    {
        Instance = this;
        checkScripts = GetComponentsInChildren<CheckCorrect>(true);

        // 保存初始位置
        foreach (var c in checkScripts)
        {
            RectTransform rt = c.GetComponent<RectTransform>();
            if (!originalPositions.ContainsKey(c))
                originalPositions.Add(c, rt.anchoredPosition);
        }
    }

    /// <summary>
    /// 某张图片完成一次有效交互后调用，只检查那张
    /// </summary>
    public bool CheckSingle(CheckCorrect checker)
    {
        return checker.CheckNow();
    }

    public bool IsPieceCorrect(CheckCorrect checker)
    {
        return checker.CheckNow();
    }

    public CheckCorrect[] GetAllCheckScripts()
    {
        return FindObjectsOfType<CheckCorrect>(true);
    }


    /// <summary>
    /// UI按钮调用，检查所有可交互图片
    /// </summary>
    public void CheckAll()
    {
        bool hasError = false;

        foreach (var c in checkScripts)
        {
            if (!c.isInteractable)
                continue;

            if (!c.CheckNow())
            {
                hasError = true;
                break;
            }
        }

        if (hasError==false)
        {
            uiManager.ShowSuccess();
            return;
        }
        else if (canDrag)   // 第三关
        {
            uiManager.ShowPartialError();
        }
        else           // 第一 / 第二关
        {
            uiManager.ShowUnfinished();
        }
    }

    // ✅ 新增接口：获取拼图原始位置
    public Vector2 GetOriginalPosition(CheckCorrect piece)
    {
        if (originalPositions.ContainsKey(piece))
            return originalPositions[piece];
        return Vector2.zero;
    }

    // ✅ 新增接口：检查拼图位置是否正确
    public bool CheckPosition(CheckCorrect piece)
    {
        if (!originalPositions.ContainsKey(piece)) return false;

        Vector2 currentPos = piece.GetComponent<RectTransform>().anchoredPosition;
        Vector2 originalPos = originalPositions[piece];

        bool xOk = Mathf.Abs(currentPos.x - originalPos.x) <= positionTolerance;
        bool yOk = Mathf.Abs(currentPos.y - originalPos.y) <= positionTolerance;

        return xOk && yOk;
    }

    // ✅ 新增接口：重置所有拼图
    public void ResetAllPieces()
    {
        foreach (var c in checkScripts)
        {
            if (!c.isInteractable) continue;

            RectTransform rt = c.GetComponent<RectTransform>();

            // 重置缩放
            rt.localScale = Vector3.one * c.initialScale;

            // 重置位置
            rt.anchoredPosition = GetOriginalPosition(c);
        }

        // 重置 MouseInteraction 内部状态
        if (mouseInteraction != null)
            mouseInteraction.ResetDraggingState();

        // 隐藏 UI
        if (uiManager != null)
        {
            uiManager.successUI.SetActive(false);
            uiManager.unfinishedUI.SetActive(false);
            uiManager.partialErrorUI.SetActive(false);
        }
    }
}

[System.Serializable]
public class PuzzleData
{
    public GameObject obj;
    public bool isInteractable;
}
