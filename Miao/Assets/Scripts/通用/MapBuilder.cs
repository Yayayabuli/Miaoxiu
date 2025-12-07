using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapBuilder : MonoBehaviour
{
    public RectTransform slotPrefab;       // Slot 预制体（带 Image+Mask）
    public RectTransform puzzleArea;       // 放 Slot 的区域
    public int rows = 2;                    // 行数
    public int cols = 2;                    // 列数
    public Vector2 cellSize = new Vector2(300, 300);
    public Vector2 spacing = new Vector2(20, 20);

#if UNITY_EDITOR
    [ContextMenu("Generate 2x2 Slots")]
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

        // ⬇ 计算整个拼图的总宽高（包含 spacing）
        float totalWidth = cols * cellSize.x + (cols - 1) * spacing.x;
        float totalHeight = rows * cellSize.y + (rows - 1) * spacing.y;

        // ⬇ 计算左上角的起点，让拼图矩阵居中
        float startX = -totalWidth * 0.5f + cellSize.x * 0.5f;
        float startY = totalHeight * 0.5f - cellSize.y * 0.5f;

        // 创建 slot
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                RectTransform slot = Instantiate(slotPrefab, puzzleArea);
                slot.name = $"Slot_{r * cols + c}";

                slot.sizeDelta = cellSize;

                // ⬇ 最正确的 UI 坐标方式
                float x = startX + c * (cellSize.x + spacing.x);
                float y = startY - r * (cellSize.y + spacing.y);

                slot.anchoredPosition = new Vector2(x, y);
            }
        }
    }
#endif

}
