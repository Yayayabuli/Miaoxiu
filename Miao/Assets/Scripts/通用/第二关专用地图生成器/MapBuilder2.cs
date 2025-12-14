using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapBuilder2 : MonoBehaviour
{
    [Header("Slot 预制体（带 Image+Mask）")]
    public RectTransform slotPrefab;

    [Header("PuzzleArea 父物体")]
    public RectTransform puzzleArea;

    [Header("行列数")]
    public int rows = 3;
    public int cols = 3;

#if UNITY_EDITOR
    [ContextMenu("Generate Slots and Pieces")]
    void GenerateSlots()
    {
        if (slotPrefab == null || puzzleArea == null)
        {
            Debug.LogError("缺少 slotPrefab 或 puzzleArea");
            return;
        }

        // 清空旧 Slot
        for (int i = puzzleArea.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(puzzleArea.GetChild(i).gameObject);
        }

        // 自动计算每格大小（无缝）
        float cellWidth = puzzleArea.rect.width / cols;
        float cellHeight = puzzleArea.rect.height / rows;

        // 左上角起点
        float startX = -puzzleArea.rect.width * 0.5f + cellWidth * 0.5f;
        float startY = puzzleArea.rect.height * 0.5f - cellHeight * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // 创建 Slot
                RectTransform slot = Instantiate(slotPrefab, puzzleArea);
                slot.name = $"Slot_{r * cols + c}";
                slot.sizeDelta = new Vector2(cellWidth + 0.5f, cellHeight + 0.5f); // 微调防止缝隙

                float x = startX + c * cellWidth;
                float y = startY - r * cellHeight;
                slot.anchoredPosition = new Vector2(x, y);

                // 可选：创建 Piece 并自动归中
                if (slotPrefab.GetComponentInChildren<CheckCorrect>() == null)
                {
                    GameObject pieceGO = new GameObject($"Piece_{r * cols + c}", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(CheckCorrect));
                    pieceGO.transform.SetParent(slot);
                    RectTransform rt = pieceGO.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.sizeDelta = Vector2.zero;
                    rt.anchoredPosition = Vector2.zero;
                }
            }
        }

        Debug.Log($"生成完成：{rows}x{cols} 无缝 Slot 和 Piece");
    }
#endif
}
