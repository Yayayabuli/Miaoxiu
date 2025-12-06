using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
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

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
        checkScripts = GetComponentsInChildren<CheckCorrect>(true);
    }

    /// <summary>
    /// 某张图片完成一次有效交互后调用，只检查那张
    /// </summary>
    public void CheckSingle(CheckCorrect checker)
    {
        bool ok = checker.CheckNow();
        if (!ok)
        {
            uiManager.ShowPartialError();
        }
    }

    /// <summary>
    /// UI按钮调用，检查所有可交互图片
    /// </summary>
    public void CheckAll()
    {
        foreach (var c in checkScripts)
        {
            if (c.isInteractable)
            {
                if (!c.CheckNow())
                {
                    uiManager.ShowUnfinished();
                    return;
                }
            }
        }

        uiManager.ShowSuccess();
    }
}

[System.Serializable]
public class PuzzleData
{
    public GameObject obj;
    public bool isInteractable;
}
